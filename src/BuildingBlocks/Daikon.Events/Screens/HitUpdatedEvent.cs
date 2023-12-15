
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class HitUpdatedEvent : BaseEvent
    {
        public HitUpdatedEvent() : base(nameof(HitUpdatedEvent))
        {
            
        }
    }
}