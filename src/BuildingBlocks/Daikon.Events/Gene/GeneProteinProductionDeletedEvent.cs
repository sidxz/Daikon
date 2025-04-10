
using Daikon.EventStore.Event;

namespace Daikon.Events.Gene
{
    public class GeneProteinProductionDeletedEvent : BaseEvent
    {
        public GeneProteinProductionDeletedEvent() : base(nameof(GeneProteinProductionDeletedEvent))
        {

        }

        public Guid GeneId { get; set; }
        public Guid ProteinProductionId { get; set; }

        
    }
}