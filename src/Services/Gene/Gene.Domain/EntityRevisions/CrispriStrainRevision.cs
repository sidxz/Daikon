
using CQRS.Core.Domain.Historical;

namespace Gene.Domain.EntityRevisions
{
    public class CrispriStrainRevision : BaseVersionEntity
    {
        public Guid GeneId { get; set; }
        public DVariableHistory<string> CrispriStrainName { get; set; }
        public DVariableHistory<string>? Notes { get; set; }
    }
}