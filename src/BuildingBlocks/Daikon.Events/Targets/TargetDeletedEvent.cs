
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Targets
{
    public class TargetDeletedEvent : BaseEvent
    {
        public TargetDeletedEvent() : base(nameof(TargetDeletedEvent))
        {
            
        }

        public string Name { get; set; }
        
    }
}