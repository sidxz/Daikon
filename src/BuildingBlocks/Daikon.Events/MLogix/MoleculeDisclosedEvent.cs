
using CQRS.Core.Event;

namespace Daikon.Events.MLogix
{
    public class MoleculeDisclosedEvent : BaseEvent
    {
        public MoleculeDisclosedEvent() : base(nameof(MoleculeDisclosedEvent))
        {

        }
        public Guid RegistrationId { get; set; }
        public string Name { get; set; }
        public string RequestedSMILES { get; set; }
        public string SmilesCanonical { get; set; }
        public Dictionary<string, string> Ids { get; set; }
    }
}