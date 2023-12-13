
using CQRS.Core.Domain.Historical;

namespace Gene.Domain.EntityRevisions
{
    public class ResistanceMutationRevision : BaseVersionEntity
    {
        public Guid GeneId { get; set; }
        public DVariableHistory<string> Mutation { get; set; }
        public DVariableHistory<string>? Isolate { get; set; }
        public DVariableHistory<string>? ParentStrain { get; set; }
        public DVariableHistory<string>? Compound { get; set; }
        public DVariableHistory<string>? ShiftInMIC { get; set; }
        public DVariableHistory<string>? Organization { get; set; }
        public DVariableHistory<string>? Researcher { get; set; }
        public DVariableHistory<string>? Reference { get; set; }
        public DVariableHistory<string>? Notes { get; set; }
    }
}