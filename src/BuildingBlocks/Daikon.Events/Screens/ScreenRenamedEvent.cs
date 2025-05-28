
using Daikon.EventStore.Event;

namespace Daikon.Events.Screens
{
    public class ScreenRenamedEvent : BaseEvent
    {
        public ScreenRenamedEvent() : base(nameof(ScreenRenamedEvent))
        {

        }
        public required string Name { get; set; }
    }
}