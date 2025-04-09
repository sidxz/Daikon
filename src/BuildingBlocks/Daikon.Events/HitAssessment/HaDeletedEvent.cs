
using Daikon.EventStore.Event;

namespace Daikon.Events.HitAssessment
{
    public class HaDeletedEvent : BaseEvent
    {
        public HaDeletedEvent() : base(nameof(HaDeletedEvent))
        {

        }

    }
}