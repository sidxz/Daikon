using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using Daikon.Shared.VM.Horizon;
using MediatR;

namespace Horizon.Application.Features.Queries.CompoundRelations
{
    public class CompoundRelationsQuery : BaseQuery, IRequest<List<CompoundRelationsVM>>
    {
        public Guid Id { get; set; }
    }
}