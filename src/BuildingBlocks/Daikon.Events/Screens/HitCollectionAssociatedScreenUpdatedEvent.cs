
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class HitCollectionAssociatedScreenUpdatedEvent : BaseEvent
    {
        public HitCollectionAssociatedScreenUpdatedEvent() : base(nameof(HitCollectionAssociatedScreenUpdatedEvent))
        {
            
        }


        public required string Name { get; set; }
        public Dictionary<string, string>? AssociatedTargets { get; set; }
    }
}