
using CQRS.Core.Event;

namespace Daikon.Events.Project
{
    public class ProjectDeletedEvent : BaseEvent
    {
        public ProjectDeletedEvent() : base(nameof(ProjectDeletedEvent))
        {

        }

    }
}