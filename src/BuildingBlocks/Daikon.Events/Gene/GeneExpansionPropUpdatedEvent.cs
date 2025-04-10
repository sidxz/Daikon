
using CQRS.Core.Domain;
using Daikon.EventStore.Event;

namespace Daikon.Events.Gene
{
    public class GeneExpansionPropUpdatedEvent : BaseEvent
    {
        public GeneExpansionPropUpdatedEvent() : base(nameof(GeneExpansionPropUpdatedEvent))
        {

        }
        public Guid ExpansionPropId { get; set; }
        public DVariable<string> ExpansionValue { get; set; }
    }
}