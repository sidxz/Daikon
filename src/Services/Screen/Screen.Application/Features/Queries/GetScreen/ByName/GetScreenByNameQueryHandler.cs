
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Daikon.Shared.VM.Screen;
namespace Screen.Application.Features.Queries.GetScreen.ByName
{
    public class GetScreenByNameQueryHandler : IRequestHandler<GetScreenByNameQuery, ScreenVM>
    {
        private readonly IMediator _mediator;
        private readonly IScreenRepository _screenRepository;
        private readonly IHitCollectionRepository _hitCollectionRepository;
        private readonly ILogger<GetScreenByNameQueryHandler> _logger;
        private readonly IMapper _mapper;

        public GetScreenByNameQueryHandler(IMediator mediator, IScreenRepository screenRepository, IHitCollectionRepository hitCollectionRepository, ILogger<GetScreenByNameQueryHandler> logger, IMapper mapper)
        {
            _mediator = mediator;
            _screenRepository = screenRepository;
            _hitCollectionRepository = hitCollectionRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ScreenVM> Handle(GetScreenByNameQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var screen = await _screenRepository.ReadScreenByName(request.Name);
                var screenVm = _mapper.Map<ScreenVM>(screen, opts => opts.Items["WithMeta"] = request.WithMeta);

                var trackableEntities = new List<VMMeta> { screenVm };
                trackableEntities.AddRange(screenVm.ScreenRuns);

                (screenVm.PageLastUpdatedDate, screenVm.PageLastUpdatedUser) = VMUpdateTracker.CalculatePageLastUpdated(trackableEntities);

                return screenVm;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetScreenByNameQueryHandler");
                throw;
            }

        }
    }
}