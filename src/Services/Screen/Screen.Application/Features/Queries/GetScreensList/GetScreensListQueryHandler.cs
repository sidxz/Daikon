using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Daikon.Shared.VM.Screen;
namespace Screen.Application.Features.Queries.GetScreensList
{
    public class GetScreensListQueryHandler : IRequestHandler<GetScreensListQuery, List<ScreensListVM>>
    {
        private readonly IMediator _mediator;
        private readonly IScreenRepository _screenRepository;
        private readonly ILogger<GetScreensListQueryHandler> _logger;
        private readonly IMapper _mapper;

        public GetScreensListQueryHandler(
            IMediator mediator,
            IScreenRepository screenRepository,
            ILogger<GetScreensListQueryHandler> logger,
            IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _screenRepository = screenRepository ?? throw new ArgumentNullException(nameof(screenRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ScreensListVM>> Handle(GetScreensListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch screens from the repository and ensure it's not null
                var screens = await _screenRepository.GetScreensList() ?? [];


                // Map screens to ViewModel with metadata
                var screensListVm = _mapper.Map<List<ScreensListVM>>(screens, opts =>
                    opts.Items["WithMeta"] = request.WithMeta) ?? new List<ScreensListVM>();

                foreach (var screenVm in screensListVm)
                {
                    // Flatten associated targets into a comma-separated string
                    screenVm.AssociatedTargetsFlattened = screenVm.AssociatedTargets?.Any() == true
                        ? string.Join(", ", screenVm.AssociatedTargets.Select(target => target.Value))
                        : string.Empty;

                    // Prepare entities for last update tracking
                    var trackableEntities = new List<VMMeta> { screenVm };
                    if (screenVm.ScreenRuns?.Any() == true)
                    {
                        trackableEntities.AddRange(screenVm.ScreenRuns);
                    }

                    // Calculate the last updated details
                    (screenVm.PageLastUpdatedDate, screenVm.PageLastUpdatedUser) =
                        VMUpdateTracker.CalculatePageLastUpdated(trackableEntities);
                }

                return screensListVm;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving screen list.");
                return new List<ScreensListVM>(); // Return empty list to avoid failures
            }
        }
    }
}
