
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class HitCreatedEvent : BaseEvent
    {
        public HitCreatedEvent() : base(nameof(HitCreatedEvent))
        {
            
        }
    }
}