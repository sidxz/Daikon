
using CQRS.Core.Query;
using MediatR;
using Target.Domain.Entities;

namespace Target.Application.Features.Queries.GetTPQ
{
    public class GetTPQQuery : BaseQuery, IRequest<PQResponse>
    {
        public Guid Id { get; set; }
    }
}