using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Horizon.Application.Features.Queries.CompoundRelations
{
    public class CompoundRelationsMultipleVM
    {
        public Dictionary<Guid, List<CompoundRelationsVM>> Relations { get; set; }
    }
}