
using CQRS.Core.Domain;

namespace Gene.Domain.Entities
{
    public class ResistanceMutation : BaseEntity
    {

        public Guid GeneId { get; set; }
        public Guid ResistanceMutationId { get; set; }

        public required DVariable<string> Mutation { get; set; }
        public DVariable<string>? Isolate { get; set; }
        public DVariable<string>? ParentStrain { get; set; }
        public DVariable<string>? Compound { get; set; }
        public DVariable<string>? ShiftInMIC { get; set; }
        public DVariable<string>? Organization { get; set; }
        public DVariable<string>? Researcher { get; set; }
        public DVariable<string>? Reference { get; set; }
        public DVariable<string>? Notes { get; set; }

    }
}