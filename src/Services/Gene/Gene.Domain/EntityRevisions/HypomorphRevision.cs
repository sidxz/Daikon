
using CQRS.Core.Domain.Historical;

namespace Gene.Domain.EntityRevisions
{
    public class HypomorphRevision : BaseVersionEntity
    {
        public Guid GeneId { get; set; }
        public DVariableHistory<string> KnockdownStrain { get; set; }
        public DVariableHistory<string>? Phenotype { get; set; }
        public DVariableHistory<string>? Notes { get; set; }
    }
}