
using CQRS.Core.Query;
using Daikon.Shared.VM.Screen;
using MediatR;

namespace Screen.Application.Features.Queries.GetHitCollection.ById
{
    public class GetHitCollectionByIdQuery : BaseQuery, IRequest<HitCollectionVM>
    {
        public Guid Id { get; set; }
    }
}