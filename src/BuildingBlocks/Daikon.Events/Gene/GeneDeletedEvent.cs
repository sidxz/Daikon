
using CQRS.Core.Event;

namespace Daikon.Events.Gene
{
    public class GeneDeletedEvent : BaseEvent
    {
        public GeneDeletedEvent() : base(nameof(GeneDeletedEvent))
        {

        }
        public string AccessionNumber { get; set; }
    }
}