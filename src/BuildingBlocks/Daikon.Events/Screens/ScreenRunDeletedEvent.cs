
using Daikon.EventStore.Event;

namespace Daikon.Events.Screens
{
    public class ScreenRunDeletedEvent : BaseEvent
    {
        public ScreenRunDeletedEvent() : base(nameof(ScreenRunDeletedEvent))
        {

        }

        public Guid ScreenRunId { get; set; }
    }
}