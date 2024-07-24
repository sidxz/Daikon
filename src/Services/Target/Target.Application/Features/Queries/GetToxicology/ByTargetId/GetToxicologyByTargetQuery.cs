using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using MediatR;
using Target.Application.Features.Queries.CommonVMs;

namespace Target.Application.Features.Queries.GetToxicology.ByTargetId
{
    public class GetToxicologyByTargetQuery : BaseQuery, IRequest<List<ToxicologyVM>>
    {
        public Guid TargetId { get; set; }
    }
}