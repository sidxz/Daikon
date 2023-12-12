
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Gene
{
    public class GeneCrispriStrainUpdatedEvent : BaseEvent
    {
        public GeneCrispriStrainUpdatedEvent() : base(nameof(GeneCrispriStrainUpdatedEvent))
        {

        }

        public Guid GeneId { get; set; }
        public Guid CrispriStrainId { get; set; }
        public required DVariable<string> CrispriStrainName { get; set; }
        public DVariable<string>? Notes { get; set; }
        
        public DateTime DateUpdated { get; set; }

    }
}