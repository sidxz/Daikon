
using Daikon.EventStore.Event;

namespace Daikon.Events.Gene
{
    public class GeneHypomorphDeletedEvent : BaseEvent
    {
        public GeneHypomorphDeletedEvent() : base(nameof(GeneHypomorphDeletedEvent))
        {
        }

        public Guid GeneId { get; set; }
        public Guid HypomorphId { get; set; }
    }
}