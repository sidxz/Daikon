
using AutoMapper;
using CQRS.Core.Extensions;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.Features.Commands.RegisterMoleculeBatch;
using MLogix.Domain.Aggregates;

namespace MLogix.Application.Features.Commands.ReregisterVault
{
    public class ReregisterVaultHandler : IRequestHandler<ReRegisterVaultCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ReregisterVaultHandler> _logger;
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IEventSourcingHandler<MoleculeAggregate> _moleculeEventSourcingHandler;
        private readonly IMoleculeAPI _moleculeApi;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReregisterVaultHandler(
            IMapper mapper,
            ILogger<ReregisterVaultHandler> logger,
            IMoleculeRepository moleculeRepository,
            IEventSourcingHandler<MoleculeAggregate> moleculeEventSourcingHandler,
            IMoleculeAPI moleculeApi,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
            _moleculeEventSourcingHandler = moleculeEventSourcingHandler ?? throw new ArgumentNullException(nameof(moleculeEventSourcingHandler));
            _moleculeApi = moleculeApi ?? throw new ArgumentNullException(nameof(moleculeApi));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Unit> Handle(ReRegisterVaultCommand request, CancellationToken cancellationToken)
        {
            var headers = GetRequestHeaders();
            var molecules = await _moleculeRepository.GetAllMolecules();

            // Prepare batch requests from molecules
            var batchRequests = molecules.Select(molecule => new RegisterMoleculeCommandWithRegId
            {
                Id = molecule.RegistrationId,
                Name = molecule.Name,
                SMILES = molecule.RequestedSMILES
            }).ToList();

            // Group molecules into batches of 500
            var batches = batchRequests.Batch(500);

            // Process each batch
            foreach (var batch in batches)
            {
                await ProcessBatchAsync(batch, headers, cancellationToken);
            }

            return Unit.Value;
        }

        private Dictionary<string, string> GetRequestHeaders()
        {
            return _httpContextAccessor.HttpContext?.Request?.Headers
                   .ToDictionary(header => header.Key, header => header.Value.ToString()) ?? new Dictionary<string, string>();
        }

        private async Task ProcessBatchAsync(List<RegisterMoleculeCommandWithRegId> batch, Dictionary<string, string> headers, CancellationToken cancellationToken)
        {
            try
            {
                var registrationResponses = await _moleculeApi.RegisterBatch(batch, headers);
                _logger.LogInformation("Successfully registered a batch of {Count} molecules.", batch.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering a batch of molecules.");
                // Re-throw to maintain existing behavior, adjust based on requirements (e.g., retry logic, skipping batches, etc.)
                throw new InvalidOperationException("An error occurred while registering a batch of molecules.", ex);
            }
        }
    }
}
