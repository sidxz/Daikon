using Daikon.Shared.APIClients.HitAssessment;
using Daikon.Shared.APIClients.Project;
using Daikon.Shared.APIClients.Screen;
using Daikon.Shared.VM.HitAssessment;
using Daikon.Shared.VM.Horizon;
using Daikon.Shared.VM.Project;
using Daikon.Shared.VM.Screen;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Aggregators.Application.Molecule.Relationships
{
    /*
     * Handler for processing DeepFindRelQuery.
     * Retrieves and aggregates molecule-related data from HitCollection, HitAssessment, and Project sources,
     * applying caching and filtering by molecule ID.
     */
    public class DeepFindRelHandler : IRequestHandler<DeepFindRelQuery, MoleculeRelationshipsVM>
    {
        private readonly IScreenAPI _screenAPIService;
        private readonly IHitAssessmentAPI _hitAssessmentAPIService;
        private readonly IProjectAPI _projectAPIService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<DeepFindRelHandler> _logger;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        public DeepFindRelHandler(
            IScreenAPI screenAPIService,
            IHitAssessmentAPI hitAssessmentAPIService,
            IProjectAPI projectAPIService,
            IMemoryCache cache,
            ILogger<DeepFindRelHandler> logger)
        {
            _screenAPIService = screenAPIService ?? throw new ArgumentNullException(nameof(screenAPIService));
            _hitAssessmentAPIService = hitAssessmentAPIService ?? throw new ArgumentNullException(nameof(hitAssessmentAPIService));
            _projectAPIService = projectAPIService ?? throw new ArgumentNullException(nameof(projectAPIService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MoleculeRelationshipsVM> Handle(DeepFindRelQuery request, CancellationToken cancellationToken)
        {
            if (request.MoleculeId == Guid.Empty)
            {
                _logger.LogWarning("Invalid MoleculeId received in DeepFindRelQuery.");
                return new MoleculeRelationshipsVM();
            }

            var result = new MoleculeRelationshipsVM
            {
                Hits = new(),
                HaCompoundEvolutions = new(),
                ProjectCompoundEvolution = new()
            };

            await Task.WhenAll(
                LoadHitsAsync(request, result),
                LoadHitAssessmentsAsync(request, result),
                LoadProjectsAsync(request, result)
            );

            return result;
        }

        private async Task LoadHitsAsync(DeepFindRelQuery request, MoleculeRelationshipsVM result)
        {
            var hitCollectionIds = GetRelationIds(request.HorizonRelations, "HitCollection");
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

                    var hits = hitCollection?.Hits?
                        .Where(h => h.MoleculeId == request.MoleculeId)
                        .ToList();

                    if (hits?.Any() == true)
                    {
                        result.Hits.AddRange(hits);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving HitCollection {HitCollectionId}", hitCollectionId);
                }
            }
        }

        private async Task LoadHitAssessmentsAsync(DeepFindRelQuery request, MoleculeRelationshipsVM result)
        {
            var hitAssessmentIds = GetRelationIds(request.HorizonRelations, "HitAssessment");
            foreach (var hitAssessmentId in hitAssessmentIds)
            {
                try
                {
                    var cacheKey = $"HitAssessment_{hitAssessmentId}";
                    if (!_cache.TryGetValue(cacheKey, out HitAssessmentVM hitAssessment))
                    {
                        hitAssessment = await _hitAssessmentAPIService.GetById(hitAssessmentId);
                        if (hitAssessment != null)
                        {
                            _cache.Set(cacheKey, hitAssessment, CacheDuration);
                        }
                    }

                    var evolutions = hitAssessment?.HaCompoundEvolution?
                        .Where(e => e.MoleculeId == request.MoleculeId)
                        .ToList();

                    if (evolutions?.Any() == true)
                    {
                        result.HaCompoundEvolutions.AddRange(evolutions);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving HitAssessment {HitAssessmentId}", hitAssessmentId);
                }
            }
        }

        private async Task LoadProjectsAsync(DeepFindRelQuery request, MoleculeRelationshipsVM result)
        {
            var projectIds = GetRelationIds(request.HorizonRelations, "Project");
            foreach (var projectId in projectIds)
            {
                try
                {
                    var cacheKey = $"Project_{projectId}";
                    if (!_cache.TryGetValue(cacheKey, out ProjectVM project))
                    {
                        project = await _projectAPIService.GetById(projectId);
                        if (project != null)
                        {
                            _cache.Set(cacheKey, project, CacheDuration);
                        }
                    }

                    var evolutions = project?.CompoundEvolution?
                        .Where(e => e.MoleculeId == request.MoleculeId)
                        .ToList();

                    if (evolutions?.Any() == true)
                    {
                        result.ProjectCompoundEvolution.AddRange(evolutions);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving Project {ProjectId}", projectId);
                }
            }
        }

        private static List<Guid> GetRelationIds(IEnumerable<CompoundRelationsVM>? relations, string nodeType) =>
            relations?
                .Where(r => r.NodeType == nodeType && r.Id != null)
                .Select(r => r.Id)
                .Distinct()
                .ToList()
            ?? new();
    }
}
