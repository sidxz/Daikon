
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class ScreenAssociatedTargetsUpdatedEvent : BaseEvent
    {
        public ScreenAssociatedTargetsUpdatedEvent() : base(nameof(ScreenAssociatedTargetsUpdatedEvent))
        {
            
        }


        public required string Name { get; set; }
        public Dictionary<string, string>? AssociatedTargets { get; set; }
    }
}