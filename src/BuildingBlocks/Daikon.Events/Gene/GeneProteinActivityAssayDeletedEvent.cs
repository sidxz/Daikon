
using Daikon.EventStore.Event;

namespace Daikon.Events.Gene
{
    public class GeneProteinActivityAssayDeletedEvent : BaseEvent
    {
        public GeneProteinActivityAssayDeletedEvent() : base(nameof(GeneProteinActivityAssayDeletedEvent))
        {
        }

        public Guid GeneId { get; set; }
        public Guid ProteinActivityAssayId { get; set; }
    }
}