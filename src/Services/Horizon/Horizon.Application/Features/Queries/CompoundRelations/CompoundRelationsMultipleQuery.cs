using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using MediatR;

namespace Horizon.Application.Features.Queries.CompoundRelations
{
    public class CompoundRelationsMultipleQuery : BaseQuery, IRequest<CompoundRelationsMultipleVM>
    {
        public List<Guid> Ids { get; set; }
    }
}