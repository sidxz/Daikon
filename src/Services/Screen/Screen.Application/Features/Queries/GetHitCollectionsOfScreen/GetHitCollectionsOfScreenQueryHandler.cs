using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Application.Features.Queries.ViewModels;

namespace Screen.Application.Features.Queries.GetHitCollectionsOfScreen
{
    public class GetHitCollectionsOfScreenQueryHandler : IRequestHandler<GetHitCollectionsOfScreenQuery, List<HitCollectionVM>>
    {
        private readonly ILogger<GetHitCollectionsOfScreenQueryHandler> _logger;
        private readonly IMediator _mediator;
        private readonly IHitCollectionRepository _hitCollectionRepository;

        public GetHitCollectionsOfScreenQueryHandler(ILogger<GetHitCollectionsOfScreenQueryHandler> logger, IMediator mediator, IHitCollectionRepository hitCollectionRepository)
        {
            _logger = logger;
            _mediator = mediator;
            _hitCollectionRepository = hitCollectionRepository;
        }

        public async Task<List<HitCollectionVM>> Handle(GetHitCollectionsOfScreenQuery request, CancellationToken cancellationToken)
        {
            var hitCollections = await _hitCollectionRepository.GetHitCollectionsListByScreenId(request.ScreenId);
            
            var hitCollectionVms = hitCollections.Select(async hc =>
            {
                var hitCollectionVm = await _mediator.Send(new GetHitCollection.ById.GetHitCollectionByIdQuery { Id = hc.Id, WithMeta = request.WithMeta });
                return hitCollectionVm;
            }).Select(t => t.Result).ToList();

            return hitCollectionVms;
        }
    }
}