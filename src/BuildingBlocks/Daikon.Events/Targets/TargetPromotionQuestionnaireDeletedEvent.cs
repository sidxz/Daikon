
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Targets
{
    public class TargetPromotionQuestionnaireDeletedEvent : BaseEvent
    {
        public TargetPromotionQuestionnaireDeletedEvent() : base(nameof(TargetPromotionQuestionnaireDeletedEvent))
        {
            
        }
        
    }
}