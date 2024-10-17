
using CQRS.Core.Query;
using MediatR;
using Target.Application.Features.Queries.GetTPQ.VMs;
using Target.Domain.Entities;

namespace Target.Application.Features.Queries.GetTPQ.ById
{
    public class GetTPQQuery : BaseQuery, IRequest<PQResponseVM>
    {
        public Guid Id { get; set; }
    }
}