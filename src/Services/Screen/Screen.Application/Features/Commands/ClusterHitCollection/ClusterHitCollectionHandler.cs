using AutoMapper;
using Daikon.EventStore.Handlers;
using Daikon.Shared.APIClients.MLogix;
using Daikon.Shared.DTO.MLogix;
using Daikon.Shared.VM.MLogix;
using Daikon.Shared.VM.Screen;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Application.Features.Commands.UpdateHit;
using Screen.Application.Features.Commands.UpdateHitBatch;
using Screen.Application.Features.Queries.GetHitCollection.ById;
using Screen.Domain.Aggregates;
using Screen.Domain.Entities;

namespace Screen.Application.Features.Commands.ClusterHitCollection
{
    /*
     * Handles clustering of hits in a HitCollection.
     * Retrieves the hit collection, clusters molecules via MLogix API,
     * updates hit cluster groups, and persists changes.
     */
    public class ClusterHitCollectionHandler : IRequestHandler<ClusterHitCollectionCommand, List<HitVM>>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ClusterHitCollectionCommand> _logger;
        private readonly IEventSourcingHandler<HitCollectionAggregate> _hitCollectionEventSourcingHandler;
        private readonly IMediator _mediator;
        private readonly IMLogixAPI _mLogixAPIService;
        private readonly IHitCollectionRepository _hitCollectionRepository;
        private readonly IHitRepository _hitRepository;

        public ClusterHitCollectionHandler(
            IMapper mapper,
            ILogger<ClusterHitCollectionCommand> logger,
            IEventSourcingHandler<HitCollectionAggregate> hitCollectionEventSourcingHandler,
            IMediator mediator,
            IMLogixAPI mLogixAPIService,
            IHitCollectionRepository hitCollectionRepository,
            IHitRepository hitRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hitCollectionEventSourcingHandler = hitCollectionEventSourcingHandler ?? throw new ArgumentNullException(nameof(hitCollectionEventSourcingHandler));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mLogixAPIService = mLogixAPIService ?? throw new ArgumentNullException(nameof(mLogixAPIService));
            _hitCollectionRepository = hitCollectionRepository ?? throw new ArgumentNullException(nameof(hitCollectionRepository));
            _hitRepository = hitRepository ?? throw new ArgumentNullException(nameof(hitRepository));
        }

        public async Task<List<HitVM>> Handle(ClusterHitCollectionCommand request, CancellationToken cancellationToken)
        {

    
            var (existingHits, moleculeDict) = await GetHitsOfCollectionAsyncNoVoteData(request.Id, cancellationToken);
            if (existingHits == null || existingHits.Count == 0)
            {
                _logger.LogWarning("No Hits found for HitCollection Id: {HitCollectionId}", request.Id);
                return null;
            }

            var moleculesForClustering = PrepareMoleculesForClustering(existingHits, moleculeDict);
            if (moleculesForClustering.Count == 0)
            {
                return null;
            }

            var clusteredHits = await GetClusteredHitsFromApiAsync(moleculesForClustering, request.Id);
            if (clusteredHits == null || clusteredHits.Count == 0)
            {
                return null;
            }

            var updateHitCommands = GenerateUpdateHitCommands(existingHits, clusteredHits, request.RequestorUserId);
            if (updateHitCommands.Count == 0)
            {
                _logger.LogWarning("No hits required updating for HitCollection Id: {HitCollectionId}", request.Id);
                return null;
            }

            var updatedHits = await PersistUpdatedHitsAsync(updateHitCommands, request.RequestorUserId, cancellationToken);
            if (updatedHits == null || updatedHits.Count == 0)
            {
                _logger.LogWarning("Failed to persist updated hits for HitCollection Id: {HitCollectionId}", request.Id);
                return null;
            }

            return updatedHits;
        }




        private async Task<(List<Hit> hits, Dictionary<Guid, MoleculeVM> moleculeDict)> GetHitsOfCollectionAsyncNoVoteData(Guid hitCollectionId, CancellationToken cancellationToken)
        {
            var hits = await _hitRepository.GetHitsListByHitCollectionId(hitCollectionId);

            if (hits == null || hits.Count == 0)
            {
                _logger.LogWarning("No Hits found for HitCollection Id: {HitCollectionId}", hitCollectionId);
                return (new List<Hit>(), new Dictionary<Guid, MoleculeVM>());
            }

            var moleculeIds = hits
                .Where(h => h.MoleculeId.HasValue)
                .Select(h => h.MoleculeId.Value)
                .Distinct()
                .ToList();

            Dictionary<Guid, MoleculeVM> moleculeDict = new();

            if (moleculeIds.Count > 0)
            {
                try
                {
                    var molecules = await _mLogixAPIService.GetMoleculesByIds(moleculeIds);

                    if (molecules != null && molecules.Any())
                    {
                        moleculeDict = molecules.ToDictionary(m => m.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching Molecules for HitCollection Id: {HitCollectionId}", hitCollectionId);
                }
            }

            return (hits, moleculeDict);
        }



        private List<ClusterDTO> PrepareMoleculesForClustering(List<Hit> hits, Dictionary<Guid, MoleculeVM> moleculeDict)
        {
            var molecules = hits
                .Where(hit => hit.MoleculeId.HasValue && moleculeDict.ContainsKey(hit.MoleculeId.Value))
                .Select(hit =>
                {
                    var molecule = moleculeDict[hit.MoleculeId.Value];
                    return new ClusterDTO
                    {
                        Id = hit.Id,
                        Name = molecule.Name,
                        SMILES = molecule.Smiles
                    };
                })
                .ToList();

            if (molecules.Count == 0)
            {
                _logger.LogWarning("No molecules found to cluster for provided Hits.");
            }

            return molecules;
        }


        private async Task<List<ClusterVM>> GetClusteredHitsFromApiAsync(List<ClusterDTO> molecules, Guid hitCollectionId)
        {
            try
            {
                var clusteredHits = await _mLogixAPIService.CalculateClusters(molecules);

                if (clusteredHits == null || clusteredHits.Count == 0)
                {
                    _logger.LogWarning("No Clustered Hits returned from API for HitCollection Id: {HitCollectionId}", hitCollectionId);
                }

                return clusteredHits;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling MLogixAPIService for HitCollection Id: {HitCollectionId}", hitCollectionId);
                return null;
            }
        }

        private List<UpdateHitCommand> GenerateUpdateHitCommands(List<Hit> existingHits, List<ClusterVM> clusteredHits, Guid requestorUserId)
        {
            var clusteredHitDictionary = clusteredHits.ToDictionary(ch => ch.Id);
            var updateCommands = new List<UpdateHitCommand>();

            foreach (var hit in existingHits)
            {
                if (!clusteredHitDictionary.TryGetValue(hit.Id, out var clusteredHit))
                {
                    continue;
                }

                hit.ClusterGroup = clusteredHit.Cluster;

                var updateCommand = _mapper.Map<UpdateHitCommand>(hit);

                updateCommand.Id = hit.HitCollectionId;
                updateCommand.HitId = hit.Id;
                updateCommand.RequestorUserId = requestorUserId;

                updateCommands.Add(updateCommand);
            }

            return updateCommands;
        }

        private async Task<List<HitVM>> PersistUpdatedHitsAsync(List<UpdateHitCommand> updateHitCommands, Guid requestorUserId, CancellationToken cancellationToken)
        {
            try
            {
                var updatedHits = await _mediator.Send(new UpdateHitBatchCommand
                {
                    RequestorUserId = requestorUserId,
                    Commands = updateHitCommands
                }, cancellationToken);

                return updatedHits;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update hits batch for RequestorUserId: {RequestorUserId}", requestorUserId);
                return null;
            }
        }
    }
}
