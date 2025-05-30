
using Daikon.EventStore.Event;

namespace Daikon.Events.Screens
{
    public class HitCollectionAssociatedScreenUpdatedEvent : BaseEvent
    {
        public HitCollectionAssociatedScreenUpdatedEvent() : base(nameof(HitCollectionAssociatedScreenUpdatedEvent))
        {

        }
        public Guid ScreenId { get; set; }

    }
}