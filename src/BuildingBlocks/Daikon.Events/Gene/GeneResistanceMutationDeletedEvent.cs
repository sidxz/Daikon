
using CQRS.Core.Event;

namespace Daikon.Events.Gene
{
    public class GeneResistanceMutationDeletedEvent : BaseEvent
    {
        public GeneResistanceMutationDeletedEvent() : base(nameof(GeneResistanceMutationDeletedEvent))
        {
        }

        public Guid GeneId { get; set; }
        public Guid ResistanceMutationId { get; set; }
    }
}