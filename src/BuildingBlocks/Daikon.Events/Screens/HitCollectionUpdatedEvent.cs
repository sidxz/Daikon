using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class HitCollectionUpdatedEvent : BaseEvent
    {
        public HitCollectionUpdatedEvent() : base(nameof(HitCollectionUpdatedEvent))
        {
            
        }

        public Guid ScreenId { get; set; }
        public required string Name { get; set; }
        public required string HitCollectionType { get; set; }
        public DVariable<string>? Notes { get; set; }
        public DVariable<string>? Owner { get; set; }
    }
}