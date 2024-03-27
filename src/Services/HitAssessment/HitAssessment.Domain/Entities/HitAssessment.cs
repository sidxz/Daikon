
using CQRS.Core.Domain;

namespace HitAssessment.Domain.Entities
{
    public class HitAssessment : BaseEntity
    {
        public Guid? StrainId { get; set; }
        public string Name { get; set; }
        public string? HaType { get; set; }
        public string? LegacyId { get; set; }
        public Guid HitId { get; set; }
        public Guid CompoundId { get; set; }

        public Dictionary<string, string> AssociatedHitIds { get; set; }
        public DVariable<DateTime> HAStart { get; set; }
        public DVariable<DateTime> HAPredictedStart { get; set; }
        public DVariable<string> HADescription { get; set; }
        public DVariable<string> HAStatus { get; set; }

        public bool IsHAComplete { get; set; }
        public DateTime? HAStatusDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? EOLDate { get; set; }
        public DateTime? CompletionDate { get; set; }

        public DVariable<Guid>? PrimaryOrgId { get; set; }
        public List<Guid>? ParticipatingOrgs { get; set; }
    }
}