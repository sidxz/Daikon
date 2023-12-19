using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class ScreenRunUpdatedEvent : BaseEvent
    {
        public ScreenRunUpdatedEvent() : base(nameof(ScreenRunUpdatedEvent))
        {

        }

        public Guid ScreenId { get; set; }
        public Guid ScreenRunId { get; set; }

        public DVariable<string> Library { get; set; }
        public DVariable<string>? Protocol { get; set; }
        public DVariable<int>? LibrarySize { get; set; }
        public DVariable<string>? Scientist { get; set; }
        public DVariable<DateTime>? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DVariable<int>? UnverifiedHitCount { get; set; }
        public DVariable<double>? HitRate { get; set; }
        public DVariable<int>? PrimaryHitCount { get; set; }
        public DVariable<int>? ConfirmedHitCount { get; set; }
        public DVariable<int>? NoOfCompoundsScreened { get; set; }
        public DVariable<string>? Concentration { get; set; }
        public DVariable<string>? ConcentrationUnit { get; set; }
        public DVariable<string>? Notes { get; set; }
    }
}