
using CQRS.Core.Event;

namespace Daikon.Events.MLogix
{
    public class MoleculeCreatedEvent : BaseEvent
    {

        public MoleculeCreatedEvent() : base(nameof(MoleculeCreatedEvent))
        {

        }
        public string Name { get; set; }
        public Guid RegistrationId { get; set; }
        public string RequestedSMILES { get; set; }
        public string SmilesCanonical { get; set; }
        public List<string> Synonyms   { get; set; }
        public Dictionary<string, string> Ids { get; set; }

    }
}