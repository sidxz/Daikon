using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class HitCollectionCreatedEvent : BaseEvent
    {
        public HitCollectionCreatedEvent() : base(nameof(HitCollectionCreatedEvent))
        {
            
        }
        public Guid HitCollectionId { get; set; }
        public Guid ScreenId { get; set; }
        public required string Name { get; set; }
        public required string HitCollectionType { get; set; }
        public DVariable<string>? Notes { get; set; }
        public DVariable<string>? Owner { get; set; }
    }
}
