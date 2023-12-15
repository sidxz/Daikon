
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class ScreenUpdatedEvent : BaseEvent
    {
        public ScreenUpdatedEvent() : base(nameof(ScreenUpdatedEvent))
        {
            
        }
    }
}