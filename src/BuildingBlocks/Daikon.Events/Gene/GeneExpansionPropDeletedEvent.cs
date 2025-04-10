
using Daikon.EventStore.Event;

namespace Daikon.Events.Gene
{
    public class GeneExpansionPropDeletedEvent : BaseEvent
    {
        public GeneExpansionPropDeletedEvent() : base(nameof(GeneExpansionPropDeletedEvent))
        {

        }
        public Guid ExpansionPropId { get; set; }
    }
}