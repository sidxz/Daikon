
using CQRS.Core.Domain.Historical;

namespace Gene.Domain.EntityRevisions
{
    public class EssentialityRevision : BaseVersionEntity
    {
        public Guid GeneId { get; set; }
        public DVariableHistory<string> Classification { get; set; }
        public DVariableHistory<string>? Condition { get; set; }
        public DVariableHistory<string>? Method { get; set; }
        public DVariableHistory<string>? Reference { get; set; }
        public DVariableHistory<string>? Note { get; set; }
    }
}