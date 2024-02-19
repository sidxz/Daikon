
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Application.Features.Queries.ViewModels;

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
            
            var screen = await _screenRepository.ReadScreenByName(request.Name);
            var screenVm = _mapper.Map<ScreenVM>(screen, opts => opts.Items["WithMeta"] = request.WithMeta);

            // var hitCollections = await _hitCollectionRepository.GetHitCollectionsListByScreenId(screen.Id);
            // screenVm.HitCollections = hitCollections.Select(async hc =>
            // {
            //     var hitCollectionVm = await _mediator.Send(new GetHitCollection.ById.GetHitCollectionByIdQuery { Id = hc.Id, WithMeta = request.WithMeta });
            //     return hitCollectionVm;
            // }).Select(t => t.Result).ToList();

            return screenVm;
            
        }
    }
}