
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class ScreenRunUpdatedEvent : BaseEvent
    {
        public ScreenRunUpdatedEvent() : base(nameof(ScreenRunUpdatedEvent))
        {
            
        }
    }
}