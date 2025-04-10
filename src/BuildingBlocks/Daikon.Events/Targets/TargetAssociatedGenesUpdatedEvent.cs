
using Daikon.EventStore.Event;

namespace Daikon.Events.Targets
{
    public class TargetAssociatedGenesUpdatedEvent: BaseEvent
    {
        public TargetAssociatedGenesUpdatedEvent() : base(nameof(TargetAssociatedGenesUpdatedEvent))
        {
            
        }
        public string Name { get; set; }
        public Dictionary<string, string> AssociatedGenes { get; set; }
        public string TargetType { get; set; }
    }
}