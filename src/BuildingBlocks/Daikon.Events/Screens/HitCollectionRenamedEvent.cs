using CQRS.Core.Domain;
using Daikon.EventStore.Event;

namespace Daikon.Events.Screens
{
    public class HitCollectionRenamedEvent : BaseEvent
    {
        public HitCollectionRenamedEvent() : base(nameof(HitCollectionRenamedEvent))
        {
            
        }
        public required string Name { get; set; }
    }
}