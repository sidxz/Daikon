
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class ScreenRunDeletedEvent : BaseEvent
    {
        public ScreenRunDeletedEvent() : base(nameof(ScreenRunDeletedEvent))
        {

        }

        public string Library { get; set; }
    }
}