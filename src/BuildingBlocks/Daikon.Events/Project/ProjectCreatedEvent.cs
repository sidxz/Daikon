
using CQRS.Core.Event;

namespace Daikon.Events.Project
{
    public class ProjectCreatedEvent : BaseEvent
    {
        public ProjectCreatedEvent() : base(nameof(ProjectCreatedEvent))
        {

        }

        public Guid? StrainId { get; set; }
        public string Name { get; set; }
        public string? ProjectType { get; set; }
        public string? LegacyId { get; set; }
        public Guid HitId { get; set; }
        public Guid CompoundId { get; set; }

        public Dictionary<string, string> AssociatedHitIds { get; set; }
        public DateTime ProjectStart { get; set; }
        public DateTime ProjectPredictedStart { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectStatus { get; set; }

        public bool IsProjectComplete { get; set; }
        public DateTime? ProjectStatusDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? EOLDate { get; set; }
        public DateTime? CompletionDate { get; set; }


        public string PrimaryOrg { get; set; }
        public List<string> SupportingOrgs { get; set; }

    }
}