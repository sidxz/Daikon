
using CQRS.Core.Event;

namespace Daikon.Events.Compounds
{
    public class CompoundRegistrationRequestedEvent : BaseEvent
    {
        public CompoundRegistrationRequestedEvent() : base(nameof(CompoundRegistrationRequestedEvent))
        {
            
        }
    }

}