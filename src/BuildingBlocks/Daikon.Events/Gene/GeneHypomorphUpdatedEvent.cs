
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Gene
{
    public class GeneHypomorphUpdatedEvent : BaseEvent
    {
        public GeneHypomorphUpdatedEvent() : base(nameof(GeneHypomorphUpdatedEvent))
        {

        }

        public Guid GeneId { get; set; }
        public Guid HypomorphId { get; set; }
        public required DVariable<string> KnockdownStrain { get; set; }
        public DVariable<string>? Phenotype { get; set; }
        public DVariable<string>? Notes { get; set; }
        
    }
}