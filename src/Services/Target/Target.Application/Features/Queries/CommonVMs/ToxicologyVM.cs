using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Target.Application.Features.Queries.CommonVMs
{
    public class ToxicologyVM
    {
        public Guid TargetId { get; set; }
        public Guid ToxicologyId { get; set; }
        public object Topic { get; set; }
        public object Impact { get; set; }
        public object ImpactPriority { get; set; }
        public object Likelihood { get; set; }
        public object LikelihoodPriority { get; set; }
        public object Note { get; set; }
    }
}