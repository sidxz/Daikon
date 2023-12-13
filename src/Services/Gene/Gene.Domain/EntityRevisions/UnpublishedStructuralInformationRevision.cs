
using CQRS.Core.Domain.Historical;

namespace Gene.Domain.EntityRevisions
{
    public class UnpublishedStructuralInformationRevision : BaseVersionEntity
    {
        public Guid GeneId { get; set; }
        public DVariableHistory<string> Organization { get; set; }
        public DVariableHistory<string>? Method { get; set; }
        public DVariableHistory<string>? Resolution { get; set; }
        public DVariableHistory<string>? Ligands { get; set; }
        public DVariableHistory<string>? Researcher { get; set; }
        public DVariableHistory<string>? Reference { get; set; }
        public DVariableHistory<string>? Notes { get; set; }
        public DVariableHistory<string>? URL { get; set; }
        
    }
}