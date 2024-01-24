
using CQRS.Core.Event;

namespace Daikon.Events.HitAssessment
{
    public class HaCreatedEvent : BaseEvent
    {
        public HaCreatedEvent() : base(nameof(HaCreatedEvent))
        {

        }

        public Guid? StrainId { get; set; }
        public string Name { get; set; }
        public string? HaType { get; set; }
        public string? LegacyId { get; set; }
        public Guid HitId { get; set; }
        public Guid CompoundId { get; set; }

        public Dictionary<string, string> AssociatedHitIds { get; set; }
        public DateTime HAStart { get; set; }
        public DateTime HAPredictedStart { get; set; }
        public string HADescription { get; set; }
        public string HAStatus { get; set; }

        public bool IsHAComplete { get; set; }
        public DateTime? HAStatusDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? EOLDate { get; set; }
        public DateTime? CompletionDate { get; set; }


        public string PrimaryOrg { get; set; }
        public List<string> SupportingOrgs { get; set; }

    }
}