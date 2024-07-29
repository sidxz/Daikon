using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain.Historical;

namespace Target.Domain.EntityRevisions
{
    public class ToxicologyRevision : BaseVersionEntity
    {
        public Guid TargetId { get; set; }
        public DVariableHistory<string> Topic { get; set; }
        public DVariableHistory<string> Impact { get; set; }
        public DVariableHistory<bool> ImpactPriority { get; set; }
        public DVariableHistory<string> Likelihood { get; set; }
        public DVariableHistory<bool> LikelihoodPriority { get; set; }
        public DVariableHistory<string> Note { get; set; }
    }
}