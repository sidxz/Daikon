using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using MediatR;

namespace Horizon.Application.Features.Queries.FindRelatedTarget
{
    public class FindRelatedTargetQuery : BaseQuery, IRequest<RelatedTargetVM>
    {
        public Guid Id { get; set; }
    }

}