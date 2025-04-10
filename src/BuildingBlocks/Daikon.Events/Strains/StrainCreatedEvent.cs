
using Daikon.EventStore.Event;

namespace Daikon.Events.Strains
{
    public class StrainCreatedEvent : BaseEvent
    {
        public StrainCreatedEvent() : base(nameof(StrainCreatedEvent))
        {

        }


        public string Name { get; set; }
        public string Organism { get; set; }

    }
}