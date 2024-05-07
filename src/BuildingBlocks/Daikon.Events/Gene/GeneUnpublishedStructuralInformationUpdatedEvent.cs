
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Gene
{
    public class GeneUnpublishedStructuralInformationUpdatedEvent : BaseEvent
    {
        public GeneUnpublishedStructuralInformationUpdatedEvent() : base(nameof(GeneUnpublishedStructuralInformationUpdatedEvent))
        {

        }

        public Guid GeneId { get; set; }
        public Guid UnpublishedStructuralInformationId { get; set; }
        public required DVariable<string> Organization { get; set; }
        public DVariable<string>? Method { get; set; }
        public DVariable<string>? Resolution { get; set; }
        public DVariable<string>? ResolutionUnit { get; set; }
        public DVariable<string>? Ligands { get; set; }
        public DVariable<string>? Researcher { get; set; }
        public DVariable<string>? Reference { get; set; }
        public DVariable<string>? Notes { get; set; }
        public DVariable<string>? URL { get; set; }


    }
}