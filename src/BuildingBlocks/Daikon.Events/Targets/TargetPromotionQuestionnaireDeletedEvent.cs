
using CQRS.Core.Domain;
using Daikon.EventStore.Event;

namespace Daikon.Events.Targets
{
    public class TargetPromotionQuestionnaireDeletedEvent : BaseEvent
    {
        public TargetPromotionQuestionnaireDeletedEvent() : base(nameof(TargetPromotionQuestionnaireDeletedEvent))
        {
            
        }
        
    }
}