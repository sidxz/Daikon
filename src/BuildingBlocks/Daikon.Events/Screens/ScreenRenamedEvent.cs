
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class ScreenRenamedEvent : BaseEvent
    {
        public ScreenRenamedEvent() : base(nameof(ScreenRenamedEvent))
        {

        }
        public Guid ScreenId { get; set; }
        public required string Name { get; set; }
    }
}