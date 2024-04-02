
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.HitAssessment
{
    public class HaUpdatedEvent : BaseEvent
    {
        public HaUpdatedEvent() : base(nameof(HaUpdatedEvent))
        {

        }

        public Guid? StrainId { get; set; }
        public string Name { get; set; }
        public string? HaType { get; set; }
        public string? LegacyId { get; set; }
        public Guid HitId { get; set; }
        public Guid CompoundId { get; set; }

        public Dictionary<string, string> AssociatedHitIds { get; set; }
        public DateTime HaStartDate { get; set; }
        public DateTime HaPredictedStartDate { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public bool IsHAComplete { get; set; }
        public DateTime? StatusDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? EOLDate { get; set; }
        public DateTime? CompletionDate { get; set; }


        public DVariable<Guid>? PrimaryOrgId { get; set; }
        public List<Guid>? ParticipatingOrgs { get; set; }
    }
}