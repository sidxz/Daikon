
using Daikon.EventStore.Event;

namespace Daikon.Events.Project
{
    public class ProjectCompoundEvolutionDeletedEvent: BaseEvent
    {
        public ProjectCompoundEvolutionDeletedEvent() : base(nameof(ProjectCompoundEvolutionDeletedEvent))
        {

        }

        public Guid CompoundEvolutionId { get; set; }
        
    }
}