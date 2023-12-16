
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class HitDeletedEvent : BaseEvent
    {
        public HitDeletedEvent() : base(nameof(HitDeletedEvent))
        {

        }
        public Guid HitCollectionId { get; set; }
        public Guid HitId { get; set; }
    }
}