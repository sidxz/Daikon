
using Daikon.EventStore.Event;

namespace Daikon.Events.Screens
{
    public class HitCollectionDeletedEvent : BaseEvent
    {
        public HitCollectionDeletedEvent() : base(nameof(HitCollectionDeletedEvent))
        {

        }
        
    }
}