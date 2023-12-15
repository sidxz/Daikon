
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class ScreenRunCreatedEvent : BaseEvent
    {
        public ScreenRunCreatedEvent() : base(nameof(ScreenRunCreatedEvent))
        {
            
        }
    }
}