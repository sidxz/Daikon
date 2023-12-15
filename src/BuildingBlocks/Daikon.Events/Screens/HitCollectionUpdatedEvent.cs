
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class HitCollectionUpdatedEvent : BaseEvent
    {
        public HitCollectionUpdatedEvent() : base(nameof(HitCollectionUpdatedEvent))
        {
            
        }
    }
}