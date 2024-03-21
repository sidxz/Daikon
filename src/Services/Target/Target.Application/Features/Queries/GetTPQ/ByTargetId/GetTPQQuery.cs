
using CQRS.Core.Query;
using MediatR;
using Target.Domain.Entities;

namespace Target.Application.Features.Queries.GetTPQ.ByTargetId
{
    public class GetTPQQuery : BaseQuery, IRequest<PQResponse>
    {
        public Guid TargetId { get; set; }
    }
}