
using Daikon.EventStore.Event;

namespace Daikon.Events.Screens
{
    public class HitDeletedEvent : BaseEvent
    {
        public HitDeletedEvent() : base(nameof(HitDeletedEvent))
        {

        }
        public Guid HitId { get; set; }
    }
}