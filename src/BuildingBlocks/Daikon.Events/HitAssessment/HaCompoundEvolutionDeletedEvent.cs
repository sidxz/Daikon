
using Daikon.EventStore.Event;

namespace Daikon.Events.HitAssessment
{
    public class HaCompoundEvolutionDeletedEvent: BaseEvent
    {
        public HaCompoundEvolutionDeletedEvent() : base(nameof(HaCompoundEvolutionDeletedEvent))
        {

        }

        public Guid CompoundEvolutionId { get; set; }
        
    }
}