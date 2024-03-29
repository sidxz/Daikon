
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Gene
{
    public class GeneUpdatedEvent : BaseEvent
    {
        public GeneUpdatedEvent() : base(nameof(GeneUpdatedEvent))
        {

        }


        public Guid StrainId { get; set; }
        public string AccessionNumber { get; set; }
        public string Name { get; set; }
        public DVariable<string> Function { get; set; }
        public DVariable<string> Product { get; set; }
        public DVariable<string> FunctionalCategory { get; set; }
        

    }
}