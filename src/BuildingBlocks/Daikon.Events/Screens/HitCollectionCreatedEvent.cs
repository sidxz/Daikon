using CQRS.Core.Domain;
using Daikon.EventStore.Event;

namespace Daikon.Events.Screens
{
    public class HitCollectionCreatedEvent : BaseEvent
    {
        public HitCollectionCreatedEvent() : base(nameof(HitCollectionCreatedEvent))
        {
            
        }
        public Guid ScreenId { get; set; }
        public required string Name { get; set; }
        public required string HitCollectionType { get; set; }
        public DVariable<string>? Notes { get; set; }
        public DVariable<string>? Owner { get; set; }
    }
}
