
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Gene
{
    public class GeneProteinActivityAssayUpdatedEvent : BaseEvent
    {
        public GeneProteinActivityAssayUpdatedEvent() : base(nameof(GeneProteinActivityAssayUpdatedEvent))
        {

        }

        public Guid GeneId { get; set; }
        public Guid ProteinActivityAssayId { get; set; }
        public required DVariable<string> Assay { get; set; }
        public DVariable<string>? Method { get; set; }
        public DVariable<string>? Throughput { get; set; }
        public DVariable<string>? PMID { get; set; }
        public DVariable<string>? Reference { get; set; }
        public DVariable<string>? URL { get; set; }
        
    }
}