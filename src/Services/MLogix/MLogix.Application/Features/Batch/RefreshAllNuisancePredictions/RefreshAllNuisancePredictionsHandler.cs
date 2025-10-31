using AutoMapper;
using Daikon.EventStore.Handlers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.DTOs.CageFusion;
using MLogix.Application.Features.Commands.PredictNuisance;
using MLogix.Domain.Aggregates;

namespace MLogix.Application.Features.Batch.RefreshAllNuisancePredictions
{
    public class RefreshAllNuisancePredictionsHandler : IRequestHandler<RefreshAllNuisancePredictionsCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<RefreshAllNuisancePredictionsHandler> _logger;
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IEventSourcingHandler<MoleculeAggregate> _moleculeEventSourcingHandler;
        private readonly IMoleculeAPI _moleculeApi;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator _mediator;

        public RefreshAllNuisancePredictionsHandler(IMapper mapper, ILogger<RefreshAllNuisancePredictionsHandler> logger, IMoleculeRepository moleculeRepository, IEventSourcingHandler<MoleculeAggregate> moleculeEventSourcingHandler, IMoleculeAPI moleculeApi, IHttpContextAccessor httpContextAccessor, IMediator mediator)
        {
            _mapper = mapper;
            _logger = logger;
            _moleculeRepository = moleculeRepository;
            _moleculeEventSourcingHandler = moleculeEventSourcingHandler;
            _moleculeApi = moleculeApi;
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(RefreshAllNuisancePredictionsCommand request, CancellationToken cancellationToken)
        {
            var headers = _httpContextAccessor.HttpContext.Request.Headers
                       .ToDictionary(h => h.Key, h => h.Value.ToString());
            request.SetUpdateProperties(request.RequestorUserId);

            // lets dump requestorId and all headers for tracing
            // _logger.LogInformation("Starting RefreshAllNuisancePredictions for RequestorUserId: {RequestorUserId}", request.RequestorUserId);
            // _logger.LogInformation("Headers: {@Headers}", headers);


            var molecules = await _moleculeRepository.GetAllRegisteredMolecules();
            // filter molecules that have a registration id and RequestedSMILES and Name

            if (molecules == null || molecules.Count == 0)
            {
                _logger.LogWarning("No registered molecules found. Nothing to refresh.");
                return Unit.Value;
            }


            // Filter: must have a resolvable SMILES and a non-empty registration.
            // Prefer canonical SMILES if available; fall back to requested.
            var eligible = molecules
                .Where(m => !string.IsNullOrWhiteSpace(m.RequestedSMILES))
                .Select(m => new NuisanceRequestTuple
                {
                    Id = m.Id.ToString(),
                    SMILES = m.RequestedSMILES!
                })
                .ToList();


            if (eligible.Count == 0)
            {
                _logger.LogWarning("Found molecules, but none with a usable SMILES/registration. Nothing to refresh.");
                return Unit.Value;
            }

            _logger.LogInformation("Prepared {Count} molecules for nuisance prediction refresh.", eligible.Count);


            var batchSize = 100;
            var interBatchDelayMs = 1000;
            var total = eligible.Count;
            var batches = Chunk(eligible, batchSize).ToList();
            var batchIndex = 0;

            foreach (var chunk in batches)
            {
                cancellationToken.ThrowIfCancellationRequested();
                batchIndex++;

                var cmd = new PredictNuisanceCommand
                {
                    NuisanceRequestTuple = chunk,
                    PlotAllAttention = false,
                    RequestorUserId = request.RequestorUserId,
                    CreatedById = request.CreatedById,
                    DateCreated = request.DateCreated,
                    IsModified = request.IsModified,
                    LastModifiedById = request.LastModifiedById,
                    DateModified = request.DateModified
                };

                

                _logger.LogInformation("Dispatching batch {Batch}/{TotalBatches} (size {BatchSize})…",
                    batchIndex, batches.Count, chunk.Count);

                try
                {
                    _logger.LogInformation("Sending PredictNuisanceCommand for batch {Batch}/{TotalBatches}", batchIndex, batches.Count);

                    await _mediator.Send(cmd, cancellationToken);
                }
                catch (Exception ex)
                {
                    // Log and continue with subsequent batches; you can change this to be fail-fast if desired.
                    _logger.LogError(ex, "PredictNuisanceCommand failed for batch {Batch}/{TotalBatches}. Continuing…",
                        batchIndex, batches.Count);
                }

                if (interBatchDelayMs > 0)
                {
                    try
                    {
                        await Task.Delay(interBatchDelayMs, cancellationToken);
                    }
                    catch (TaskCanceledException) { /* ignore on cancel */ }
                }
            }

            _logger.LogInformation("Completed nuisance prediction refresh for {Total} molecules in {Batches} batches.",
                total, batches.Count);

            return Unit.Value;



        }


        private static IEnumerable<List<T>> Chunk<T>(IReadOnlyList<T> source, int size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            var count = source.Count;
            for (int i = 0; i < count; i += size)
            {
                var take = Math.Min(size, count - i);
                var list = new List<T>(take);
                for (int j = 0; j < take; j++) list.Add(source[i + j]);
                yield return list;
            }
        }
    }
}