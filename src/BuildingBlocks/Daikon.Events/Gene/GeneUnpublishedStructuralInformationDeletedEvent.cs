
using CQRS.Core.Event;

namespace Daikon.Events.Gene
{
    public class GeneUnpublishedStructuralInformationDeletedEvent : BaseEvent
    {
        public GeneUnpublishedStructuralInformationDeletedEvent() : base(nameof(GeneUnpublishedStructuralInformationDeletedEvent))
        {
        }

        public Guid GeneId { get; set; }
        public Guid UnpublishedStructuralInformationId { get; set; }
    }
}