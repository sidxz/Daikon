
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class HitCollectionDeletedEvent : BaseEvent
    {
        public HitCollectionDeletedEvent() : base(nameof(HitCollectionDeletedEvent))
        {

        }
        public Guid ScreenId { get; set; }
        public Guid HitCollectionId { get; set; }
        public string Name { get; set; }
    }
}