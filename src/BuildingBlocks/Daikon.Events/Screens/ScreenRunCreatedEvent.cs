
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class ScreenCreatedEvent : BaseEvent
    {
        public ScreenCreatedEvent() : base(nameof(ScreenCreatedEvent))
        {
            
        }
    }
}