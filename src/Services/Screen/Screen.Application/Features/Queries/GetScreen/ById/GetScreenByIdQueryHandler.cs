
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Daikon.Shared.VM.Screen;
namespace Screen.Application.Features.Queries.GetScreen.ById
{
    public class GetScreenByIdQueryHandler : IRequestHandler<GetScreenByIdQuery, ScreenVM>
    {
        private readonly IMediator _mediator;
        private readonly IScreenRepository _screenRepository;
        private readonly IScreenRunRepository _screenRunRepository;
        private readonly IHitCollectionRepository _hitCollectionRepository;
        private readonly ILogger<GetScreenByIdQueryHandler> _logger;
        private readonly IMapper _mapper;

        public GetScreenByIdQueryHandler(IMediator mediator, IScreenRepository screenRepository, IScreenRunRepository screenRunRepository,
                        IHitCollectionRepository hitCollectionRepository, ILogger<GetScreenByIdQueryHandler> logger, IMapper mapper)
        {
            _mediator = mediator;
            _screenRepository = screenRepository;
            _hitCollectionRepository = hitCollectionRepository;
            _screenRunRepository = screenRunRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ScreenVM> Handle(GetScreenByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var screen = await _screenRepository.ReadScreenById(request.Id);
                var screenVm = _mapper.Map<ScreenVM>(screen, opts => opts.Items["WithMeta"] = request.WithMeta);

                var screenRuns = await _screenRunRepository.GetScreenRunsListByScreenId(screen.Id);
                screenVm.ScreenRuns = _mapper.Map<List<ScreenRunVM>>(screenRuns, opts => opts.Items["WithMeta"] = request.WithMeta);

                var trackableEntities = new List<VMMeta> { screenVm };
                trackableEntities.AddRange(screenVm.ScreenRuns);

                (screenVm.PageLastUpdatedDate, screenVm.PageLastUpdatedUser) = VMUpdateTracker.CalculatePageLastUpdated(trackableEntities);

                return screenVm;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetScreenByIdQueryHandler");
                throw;
            }

        }
    }
}