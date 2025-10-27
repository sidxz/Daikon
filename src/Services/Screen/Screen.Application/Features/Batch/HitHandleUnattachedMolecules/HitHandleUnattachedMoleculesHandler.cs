
using AutoMapper;
using Daikon.EventStore.Handlers;
using Daikon.Events.Screens;
using Daikon.Shared.APIClients.MLogix;
using Daikon.Shared.DTO.MLogix;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;
using Screen.Domain.Entities;

namespace Screen.Application.Features.Batch.HitHandleUnattachedMolecules
{
    public class HitHandleUnattachedMoleculesHandler : IRequestHandler<HitHandleUnattachedMoleculesCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<HitHandleUnattachedMoleculesHandler> _logger;
        private readonly IHitRepository _hitRepository;
        private readonly IMLogixAPI _mLogixApiService;
        private readonly IEventSourcingHandler<HitCollectionAggregate> _hitCollectionEventSourcingHandler;

        public HitHandleUnattachedMoleculesHandler(
            IMapper mapper,
            ILogger<HitHandleUnattachedMoleculesHandler> logger,
            IHitRepository hitRepository,
            IMLogixAPI mLogixApiService,
            IEventSourcingHandler<HitCollectionAggregate> hitCollectionEventSourcingHandler)
        {
            _mapper = mapper;
            _logger = logger;
            _hitRepository = hitRepository;
            _mLogixApiService = mLogixApiService;
            _hitCollectionEventSourcingHandler = hitCollectionEventSourcingHandler;
        }

        public async Task<Unit> Handle(HitHandleUnattachedMoleculesCommand request, CancellationToken cancellationToken)
        {
            List<Hit> unattachedHits;

            try
            {
                unattachedHits = await _hitRepository.GetHitsWithRequestedMoleculeNameButNoMoleculeId();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve unattached hits from repository.");
                throw;
            }

            if (unattachedHits == null || unattachedHits.Count == 0)
            {
                _logger.LogInformation("No unattached hits found.");
                return Unit.Value;
            }

            _logger.LogInformation("Found {Count} unattached hits to process.", unattachedHits.Count);

            foreach (var hit in unattachedHits)
            {
                _logger.LogInformation("Processing unattached hit with ID: {HitId}", hit.Id);

                try
                {
                    // Prepare registration DTO
                    var registerDto = new RegisterMoleculeDTO
                    {
                        Name = hit.RequestedMoleculeName,
                        SMILES = hit.RequestedSMILES ?? string.Empty,
                        DisclosureStage = Daikon.Shared.Constants.Workflow.Stages.Screen
                    };

                    _logger.LogInformation("Name : {Name}", registerDto.Name);
                    _logger.LogInformation("SMILES : {SMILES}", registerDto.SMILES);


                    // Attempt molecule registration
                    
                    var mLogixResponse = await _mLogixApiService.RegisterMolecule(registerDto);

                    if (mLogixResponse == null)
                    {
                        _logger.LogWarning("Molecule registration failed for hit ID {HitId}. Response was null.", hit.Id);
                        continue;
                    }

                    // Prepare domain event with registration data
                    var moleculeUpdatedEvent = new HitMoleculeUpdatedEvent
                    {
                        Id = hit.HitCollectionId,
                        HitId = hit.Id,
                        RequestedSMILES = hit.RequestedSMILES,
                        IsStructureDisclosed = !string.IsNullOrWhiteSpace(hit.RequestedSMILES?.Value),
                        MoleculeId = mLogixResponse.Id,
                        MoleculeRegistrationId = mLogixResponse.RegistrationId
                    };

                    // Load aggregate and apply event
                    var aggregate = await _hitCollectionEventSourcingHandler.GetByAsyncId(hit.HitCollectionId);
                    aggregate.HitMoleculeUpdated(moleculeUpdatedEvent);

                    await _hitCollectionEventSourcingHandler.SaveAsync(aggregate);

                    _logger.LogInformation("Successfully registered and updated hit {HitId}.", hit.Id);
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing hit ID {HitId}. Skipping to next.", hit.Id);
                }
            }

            return Unit.Value;
        }
    }
}
