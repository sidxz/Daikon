using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class ScreenUpdatedEvent : BaseEvent
    {
        public ScreenUpdatedEvent() : base(nameof(ScreenUpdatedEvent))
        {
            
        }

        public Guid? StrainId { get; set; }
        public required string Name { get; set; }
        public Dictionary<string, string>? AssociatedTargets { get; set; }
        public string? ScreenType { get; set; }
        public DVariable<string>? Method { get; set; }
        public DVariable<string>? Status { get; set; }
        public DVariable<DateTime>? LatestStatusChangeDate { get; set; }
        public DVariable<string>? Notes { get; set; }
        public DVariable<Guid>? PrimaryOrgId { get; set; }
        public DVariable<string>? PrimaryOrgName { get; set; }

        public string? Owner { get; set; }
        public DVariable<DateTime>? ExpectedCompleteDate { get; set; }

        public Dictionary<string, string>? ParticipatingOrgs { get; set; }
    }
}