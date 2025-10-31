using AutoMapper;
using Daikon.Events.MLogix;
using Daikon.EventStore.Handlers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.CageFusion;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.DTOs.CageFusion;
using MLogix.Domain.Aggregates;

namespace MLogix.Application.Features.Commands.PredictNuisance
{
    public class PredictNuisanceHandler : IRequestHandler<PredictNuisanceCommand, NuisanceResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PredictNuisanceHandler> _logger;
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IMoleculePredictionRepository _moleculePredictionRepository;
        private readonly IEventSourcingHandler<MoleculeAggregate> _moleculeEventSourcingHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMoleculeAPI _iMoleculeAPI;
        private readonly INuisanceAPI _nuisanceAPI;

        public PredictNuisanceHandler(IMapper mapper, ILogger<PredictNuisanceHandler> logger, IMoleculeRepository moleculeRepository, IMoleculePredictionRepository moleculePredictionRepository, IEventSourcingHandler<MoleculeAggregate> moleculeEventSourcingHandler, IHttpContextAccessor httpContextAccessor, IMoleculeAPI iMoleculeAPI, INuisanceAPI nuisanceAPI)
        {
            _mapper = mapper;
            _logger = logger;
            _moleculeRepository = moleculeRepository;
            _moleculePredictionRepository = moleculePredictionRepository;
            _moleculeEventSourcingHandler = moleculeEventSourcingHandler;
            _httpContextAccessor = httpContextAccessor;
            _iMoleculeAPI = iMoleculeAPI;
            _nuisanceAPI = nuisanceAPI;
        }

        public async Task<NuisanceResponseDTO> Handle(PredictNuisanceCommand request, CancellationToken cancellationToken)
        {
            var headers = _httpContextAccessor.HttpContext.Request.Headers
                       .ToDictionary(h => h.Key, h => h.Value.ToString());
            request.SetUpdateProperties(request.RequestorUserId);


            // check if request is empty
            if (request.NuisanceRequestTuple == null || request.NuisanceRequestTuple.Count == 0)
            {
                throw new ArgumentException("NuisanceRequestTuple cannot be null or empty");
            }

            NuisanceRequestDTO nuisanceRequestDTO = new NuisanceRequestDTO
            {
                PlotAllAttention = request.PlotAllAttention,
                Items = request.NuisanceRequestTuple
            };

            NuisanceResponseDTO result;
            try
            {
                result = await _nuisanceAPI.GetNuisancePredictionsAsync(nuisanceRequestDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting nuisance predictions.");
                throw;
            }

            // loop through results and we will create events
            foreach (NuisanceResponseRow row in result.Rows)
            {
                if (Guid.TryParse(row.Id, out Guid id))
                {
                    MoleculeNuisancePredictedEvent moleculeNuisancePredictedEvent = new MoleculeNuisancePredictedEvent
                    {
                        MoleculeId = id,
                        RequestedModelName = result.ModelName,
                        RequestDate = DateTime.UtcNow,
                        NuisancePrediction = new Daikon.Shared.Embedded.MLogix.Nuisance
                        {
                            Id = id,
                            ModelName = result.ModelName,
                            PredictionDate = result.TimeGenerated,
                            LabelAggregator = row.PredClassAggregator,
                            ScoreAggregator = row.Aggregator,
                            LabelLuciferaseInhibitor = row.PredClassLuciferaseInhibitor,
                            ScoreLuciferaseInhibitor = row.LuciferaseInhibitor,
                            LabelReactive = row.PredClassReactive,
                            ScoreReactive = row.Reactive,
                            LabelPromiscuous = row.PredClassPromiscuous,
                            ScorePromiscuous = row.Promiscuous,
                            IsVerified = false
                        }
                    };

                    // Save event
                    try
                    {
                        var aggregate = await _moleculeEventSourcingHandler.GetByAsyncId(id);
                        aggregate.PredictNuisance(moleculeNuisancePredictedEvent);
                        await _moleculeEventSourcingHandler.SaveAsync(aggregate);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while saving MoleculeNuisancePredictedEvent for Molecule ID: {MoleculeId}", row.Id);
                        throw;
                    }

                }
                // If parsing fails, skip this row
                else
                {
                    _logger.LogWarning("Invalid GUID format for Molecule ID: {MoleculeId}", row.Id);
                    continue;
                }
            }
            return result;
        }
    }
}