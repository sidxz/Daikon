using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using MediatR;

namespace Horizon.Application.Features.Queries.FIndTargetRelations
{
    public class FindTargetRelationsCommand : BaseQuery, IRequest<TargetRelationsVM>
    {
        public string UniId { get; set; }
    }
}