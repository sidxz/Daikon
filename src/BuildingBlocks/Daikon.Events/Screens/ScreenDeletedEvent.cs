
using Daikon.EventStore.Event;

namespace Daikon.Events.Screens
{
    public class ScreenDeletedEvent : BaseEvent
    {
        public ScreenDeletedEvent() : base(nameof(ScreenDeletedEvent))
        {

        }
    }
}