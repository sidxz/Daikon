using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daikon.Shared.VM.Horizon
{
    public class CompoundRelationsMultipleVM
    {
        public Dictionary<Guid, List<CompoundRelationsVM>> Relations { get; set; }
    }
}