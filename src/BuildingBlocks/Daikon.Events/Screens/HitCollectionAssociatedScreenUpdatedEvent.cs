
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class HitCollectionAssociatedScreenUpdatedEvent : BaseEvent
    {
        public HitCollectionAssociatedScreenUpdatedEvent() : base(nameof(HitCollectionAssociatedScreenUpdatedEvent))
        {

        }


        public string Name { get; set; }
        public Guid HitCollectionId { get; set; }
        public Guid ScreenId { get; set; }


    }
}