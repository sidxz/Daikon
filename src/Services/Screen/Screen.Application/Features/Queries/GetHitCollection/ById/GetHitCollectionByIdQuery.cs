
using CQRS.Core.Query;
using MediatR;
using Screen.Application.Features.Queries.ViewModels;

namespace Screen.Application.Features.Queries.GetHitCollection.ById
{
    public class GetHitCollectionByIdQuery : BaseQuery, IRequest<HitCollectionVM>
    {
        public Guid Id { get; set; }
    }
}