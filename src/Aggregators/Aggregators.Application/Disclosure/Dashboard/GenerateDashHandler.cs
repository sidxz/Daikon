using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aggregators.Application.Molecule.Relationships;
using AutoMapper;
using Daikon.Shared.APIClients.Horizon;
using Daikon.Shared.APIClients.MLogix;
using Daikon.Shared.APIClients.Screen;
using Daikon.Shared.VM.Horizon;
using Daikon.Shared.VM.Screen;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Aggregators.Application.Disclosure.Dashboard
{
    /*
     * Handler for generating the Disclosure Dashboard view.
     * Retrieves recent molecules, enriches them with horizon relationships and screening hits,
     * and composes a structured dashboard response.
     */
    public class GenerateDashHandler : IRequestHandler<GenerateDashQuery, DisclosureDashView>
    {
        private readonly IMLogixAPI _mLogixAPIService;
        private readonly IHorizonAPI _horizonAPIService;
        private readonly IScreenAPI _screenAPIService;
        private readonly ILogger<GenerateDashHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IMediator _mediator;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        public GenerateDashHandler(
            IMLogixAPI mLogixAPIService,
            IHorizonAPI horizonAPIService,
            IScreenAPI screenAPIService,
            ILogger<GenerateDashHandler> logger,
            IMapper mapper,
            IMediator mediator,
            IMemoryCache cache)
        {
            _mLogixAPIService = mLogixAPIService ?? throw new ArgumentNullException(nameof(mLogixAPIService));
            _horizonAPIService = horizonAPIService ?? throw new ArgumentNullException(nameof(horizonAPIService));
            _screenAPIService = screenAPIService ?? throw new ArgumentNullException(nameof(screenAPIService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<DisclosureDashView> Handle(GenerateDashQuery request, CancellationToken cancellationToken)
        {
            var disclosureDashView = new DisclosureDashView();

            try
            {
                var molecules = await _mLogixAPIService.GetRecentDisclosures(request.StartDate, request.EndDate);
                if (molecules == null || !molecules.Any())
                {
                    _logger.LogInformation("No recent disclosures found for the specified date range.");
                    return disclosureDashView;
                }

                var moleculeIds = molecules.Select(m => m.Id).ToList();

                // Compose a cache key that avoids overflow
                var cacheKey = $"CompoundRelations_{string.Join("_", moleculeIds.Take(5))}_{moleculeIds.Count}";
                if (!_cache.TryGetValue(cacheKey, out CompoundRelationsMultipleVM compoundRelations))
                {
                    compoundRelations = await _horizonAPIService.GetCompoundRelationsMultiple(moleculeIds);
                    _cache.Set(cacheKey, compoundRelations, CacheDuration);
                }

                var tableElements = new List<DisclosureDashTableElemView>();

                foreach (var molecule in molecules)
                {
                    var dashRow = _mapper.Map<DisclosureDashTableElemView>(molecule);

                    // Set horizon relations if found
                    dashRow.HorizonRelations = compoundRelations?.Relations
                        .GetValueOrDefault(molecule.Id, new List<CompoundRelationsVM>());

                    var hitQuery = new DeepFindRelQuery
                    {
                        MoleculeId = molecule.Id,
                        HorizonRelations = dashRow.HorizonRelations
                    };

                    try
                    {
                        var hitResult = await _mediator.Send(hitQuery, cancellationToken);
                        dashRow.Hits = hitResult?.Hits ?? new List<HitVM>();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to retrieve hits for molecule {MoleculeId}", molecule.Id);
                        dashRow.Hits = new List<HitVM>();
                    }

                    tableElements.Add(dashRow);
                }

                disclosureDashView.TableElements = tableElements;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the disclosure dashboard.");
               
            }

            return disclosureDashView;
        }
    }
}
