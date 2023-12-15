
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class HitDeletedEvent : BaseEvent
    {
        public HitDeletedEvent() : base(nameof(HitDeletedEvent))
        {

        }
    }
}