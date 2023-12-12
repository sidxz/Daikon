
using CQRS.Core.Event;

namespace Daikon.Events.Gene
{
    public class GeneEssentialityDeletedEvent : BaseEvent
    {
        public GeneEssentialityDeletedEvent() : base(nameof(GeneEssentialityDeletedEvent))
        {
        }

        public Guid GeneId { get; set; }
        public Guid EssentialityId { get; set; }
    }
}