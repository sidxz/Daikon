
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class ScreenAssociatedTargetUpdatedEvent : BaseEvent
    {
        public ScreenAssociatedTargetUpdatedEvent() : base(nameof(ScreenAssociatedTargetUpdatedEvent))
        {
            
        }


        public required string Name { get; set; }
        public Dictionary<string, string>? AssociatedTargets { get; set; }
    }
}