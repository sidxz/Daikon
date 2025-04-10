
using CQRS.Core.Domain;
using Daikon.EventStore.Event;

namespace Daikon.Events.Screens
{
    public class HitMoleculeUpdatedEvent : BaseEvent
    {
        public HitMoleculeUpdatedEvent() : base(nameof(HitMoleculeUpdatedEvent))
        {
            
        }

        public Guid HitId { get; set; }
        public DVariable<string>? Library { get; set; }
        public DVariable<string>? RequestedSMILES { get; set; }
        public DVariable<bool> IsStructureDisclosed { get; set; } = new DVariable<bool>(false);
        public Guid MoleculeId { get; set; }
        public Guid MoleculeRegistrationId { get; set; }

    }
}