
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class HitCollectionCreatedEvent : BaseEvent
    {
        public HitCollectionCreatedEvent() : base(nameof(HitCollectionCreatedEvent))
        {
            
        }
    }
}