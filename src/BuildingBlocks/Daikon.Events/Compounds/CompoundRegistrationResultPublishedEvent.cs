
using CQRS.Core.Event;

namespace Daikon.Events.Compounds
{
    public class CompoundRegistrationResultPublishedEvent : BaseEvent
    {
        public CompoundRegistrationResultPublishedEvent() : base(nameof(CompoundRegistrationResultPublishedEvent))
        {
            
        }
    }
}