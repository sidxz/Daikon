
using Daikon.EventStore.Event;

namespace Daikon.Events.Strains
{
    public class StrainDeletedEvent : BaseEvent
    {
        public StrainDeletedEvent() : base(nameof(StrainDeletedEvent))
        {

        }
        public string Name { get; set; }
    }
}