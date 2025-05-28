

using CQRS.Core.Domain;
using Daikon.EventStore.Event;

namespace Daikon.Events.Gene
{
    public class GeneExpansionPropAddedEvent : BaseEvent
    {
        public GeneExpansionPropAddedEvent() : base(nameof(GeneExpansionPropAddedEvent))
        {

        }
        public Guid ExpansionPropId { get; set; }
        public required string ExpansionType { get; set; }
        public DVariable<string> ExpansionValue { get; set; }

    }
}