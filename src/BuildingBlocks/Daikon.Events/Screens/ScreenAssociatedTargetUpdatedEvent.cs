
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class ScreenAssociatedTargetUpdatedEvent : BaseEvent
    {
        public ScreenAssociatedTargetUpdatedEvent() : base(nameof(ScreenAssociatedTargetUpdatedEvent))
        {
            
        }
    }
}