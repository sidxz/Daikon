using Daikon.Shared.APIClients.Screen;
using Daikon.Shared.VM.Screen;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aggregators.Application.Molecule.Relationships
{
    /*
     * Handler for processing DeepFindRelQuery. 
     * This handler retrieves detailed hit information for a molecule by expanding on provided horizon relationships,
     * using a cached approach to minimize redundant API calls to the Screen service.
     */
    public class DeepFindRelHandler : IRequestHandler<DeepFindRelQuery, MoleculeRelationshipsVM>
    {
        private readonly IScreenAPI _screenAPIService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<DeepFindRelHandler> _logger;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        public DeepFindRelHandler(
            IScreenAPI screenAPIService,
            IMemoryCache cache,
            ILogger<DeepFindRelHandler> logger)
        {
            _screenAPIService = screenAPIService;
            _cache = cache;
            _logger = logger;
        }

        /*
         * Handles the DeepFindRelQuery by retrieving HitCollection data (with caching),
         * filtering hits by MoleculeId, and constructing the final result.
         */
        public async Task<MoleculeRelationshipsVM> Handle(DeepFindRelQuery request, CancellationToken cancellationToken)
        {
            var hits = new List<HitVM>();

            // Validate molecule ID
            if (request.MoleculeId == Guid.Empty)
            {
                _logger.LogWarning("Invalid MoleculeId received in DeepFindRelQuery.");
                return new MoleculeRelationshipsVM();
            }

            // Filter relevant HitCollection IDs from input relationships
            var hitCollectionIds = request.HorizonRelations?
                .Where(r => r.NodeType == "HitCollection" && r.Id != null)
                .Select(r => r.Id)
                .Distinct()
                .ToList();

            if (hitCollectionIds == null || !hitCollectionIds.Any())
            {
                _logger.LogInformation("No HitCollection relationships found for molecule {MoleculeId}.", request.MoleculeId);
                return new MoleculeRelationshipsVM();
            }

            foreach (var hitCollectionId in hitCollectionIds)
            {
                try
                {
                    var cacheKey = $"HitCollection_{hitCollectionId}";
                    if (!_cache.TryGetValue(cacheKey, out HitCollectionVM hitCollection))
                    {
                        hitCollection = await _screenAPIService.GetHitCollectionById(hitCollectionId);

                        if (hitCollection != null)
                        {
                            _cache.Set(cacheKey, hitCollection, CacheDuration);
                        }
                    }

                    if (hitCollection?.Hits != null)
                    {
                        var matchingHits = hitCollection.Hits
                            .Where(h => h.MoleculeId == request.MoleculeId)
                            .ToList();

                        if (matchingHits.Any())
                        {
                            hits.AddRange(matchingHits);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while retrieving or processing HitCollection {HitCollectionId}", hitCollectionId);
                    // Continue with next hit collection instead of failing
                }
            }

            return new MoleculeRelationshipsVM { Hits = hits };
        }
    }
}
