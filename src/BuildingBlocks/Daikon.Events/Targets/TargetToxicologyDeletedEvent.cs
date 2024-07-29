
using CQRS.Core.Event;

namespace Daikon.Events.Targets
{
    public class TargetToxicologyDeletedEvent : BaseEvent
    {
        public TargetToxicologyDeletedEvent() : base(nameof(TargetToxicologyDeletedEvent))
        {
        }

        public Guid TargetId { get; set; }
        public Guid ToxicologyId { get; set; }

    }
}