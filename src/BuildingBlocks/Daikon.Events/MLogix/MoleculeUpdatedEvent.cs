
using Daikon.EventStore.Event;

namespace Daikon.Events.MLogix
{
    public class MoleculeUpdatedEvent : BaseEvent
    {
        public MoleculeUpdatedEvent() : base(nameof(MoleculeUpdatedEvent))
        {

        }
        public Guid RegistrationId { get; set; }
        public string Name { get; set; }
        public string RequestedSMILES { get; set; }
        public string SmilesCanonical { get; set; }
        public Dictionary<string, string> Ids { get; set; }
    }
}