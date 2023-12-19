
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class ScreenRunDeletedEvent : BaseEvent
    {
        public ScreenRunDeletedEvent() : base(nameof(ScreenRunDeletedEvent))
        {

        }
        public Guid ScreenId { get; set; }
        public Guid ScreenRunId { get; set; }
        public string Library { get; set; }
    }
}