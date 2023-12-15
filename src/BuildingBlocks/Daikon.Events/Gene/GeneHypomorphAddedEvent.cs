
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Gene
{
    public class GeneHypomorphAddedEvent : BaseEvent
    {
        public GeneHypomorphAddedEvent() : base(nameof(GeneHypomorphAddedEvent))
        {

        }


        public Guid GeneId { get; set; }
        public Guid HypomorphId { get; set; }
        public required DVariable<string> KnockdownStrain { get; set; }
        public DVariable<string>? Phenotype { get; set; }
        public DVariable<string>? Notes { get; set; }
        
        public DateTime DateCreated { get; set; }

    }
}