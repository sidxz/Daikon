
using CQRS.Core.Query;
using MediatR;

namespace Target.Application.Features.Queries.GetTarget.ById
{
    public class GetTargetByIdQuery : BaseQuery, IRequest<TargetVM>
    {
        public Guid Id { get; set; }
    }
}