using CQRS.Core.Domain;
using Daikon.EventStore.Event;

namespace Daikon.Events.Screens
{
    public class ScreenRunAddedEvent : BaseEvent
    {
        public ScreenRunAddedEvent() : base(nameof(ScreenRunAddedEvent))
        {
            
        }

        public Guid ScreenRunId { get; set; }
        public DVariable<string> Library { get; set; }
        public DVariable<string>? Protocol { get; set; }
        public DVariable<string>? LibrarySize { get; set; }
        public DVariable<string>? Scientist { get; set; }
        public DVariable<DateTime>? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DVariable<string>? UnverifiedHitCount { get; set; }
        public DVariable<string>? HitRate { get; set; }
        public DVariable<string>? PrimaryHitCount { get; set; }
        public DVariable<string>? ConfirmedHitCount { get; set; }
        public DVariable<string>? NoOfCompoundsScreened { get; set; }
        public DVariable<string>? Concentration { get; set; }
        public DVariable<string>? ConcentrationUnit { get; set; }
        public DVariable<string>? Notes { get; set; }
    }
}