

using CQRS.Core.Query;
using Daikon.Shared.VM.Screen;
using MediatR;

namespace Screen.Application.Features.Queries.GetHitCollectionsOfScreen
{
    public class GetHitCollectionsOfScreenQuery: BaseQuery, IRequest<List<HitCollectionVM>>
    {
        public Guid ScreenId { get; set; }
    }
}