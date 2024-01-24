

using CQRS.Core.Query;
using MediatR;
using Screen.Application.Features.Queries.ViewModels;

namespace Screen.Application.Features.Queries.GetHitCollectionsOfScreen
{
    public class GetHitCollectionsOfScreenQuery: BaseQuery, IRequest<List<HitCollectionVM>>
    {
        public Guid ScreenId { get; set; }
    }
}