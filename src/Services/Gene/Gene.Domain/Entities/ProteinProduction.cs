using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace Gene.Domain.Entities
{
    public class ProteinProduction : BaseEntity
    {

        public Guid GeneId { get; set; }
        public Guid ProteinProductionId { get; set; }

        public required DVariable<string> Production { get; set; }
        public DVariable<string>? Method { get; set; }
        public DVariable<string>? Purity { get; set; }
        public DVariable<DateTime>? DateProduced { get; set; }
        public DVariable<string>? PMID { get; set; }
        public DVariable<string>? Notes { get; set; }
        public DVariable<string>? URL { get; set; }
    
        
    }
}