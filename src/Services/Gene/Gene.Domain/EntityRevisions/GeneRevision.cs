
using CQRS.Core.Domain.Historical;

namespace Gene.Domain.EntityRevisions
{
    public class GeneRevision : BaseVersionEntity
    {
        
        public DVariableHistory<string> Product { get; set; }
        public DVariableHistory<string> FunctionalCategory { get; set; }
        public DVariableHistory<string> Comments { get; set; }
        public DVariableHistory<string> GeneSequence { get; set; }
        public DVariableHistory<string> ProteinSequence { get; set; }

        
    }
}