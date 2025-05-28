
using Daikon.EventStore.Event;

namespace Daikon.Events.Strains
{
    public class StrainUpdatedEvent : BaseEvent
    {
        public StrainUpdatedEvent() : base(nameof(StrainUpdatedEvent))
        {

        }


        public string Name { get; set; }
        public string Organism { get; set; }

    }
}