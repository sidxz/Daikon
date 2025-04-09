
using Daikon.EventStore.Event;

namespace Daikon.Events.Gene
{
    public class GeneCrispriStrainDeletedEvent : BaseEvent
    {
        public GeneCrispriStrainDeletedEvent() : base(nameof(GeneCrispriStrainDeletedEvent))
        {
        }

        public Guid GeneId { get; set; }
        public Guid CrispriStrainId { get; set; }
    }
}