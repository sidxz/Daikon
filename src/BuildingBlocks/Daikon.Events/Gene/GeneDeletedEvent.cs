
using Daikon.EventStore.Event;

namespace Daikon.Events.Gene
{
    public class GeneDeletedEvent : BaseEvent
    {
        public GeneDeletedEvent() : base(nameof(GeneDeletedEvent))
        {

        }
       
    }
}