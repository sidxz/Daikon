
using Daikon.Events.Targets;

namespace Target.Application.EventHandlers
{
    public interface ITargetEventHandler
    {
        Task OnEvent(TargetCreatedEvent @event);
        Task OnEvent(TargetUpdatedEvent @event);
        Task OnEvent(TargetDeletedEvent @event);
        Task OnEvent(TargetPromotionQuestionnaireSubmittedEvent @event);
        Task OnEvent(TargetPromotionQuestionnaireUpdatedEvent @event);
        Task OnEvent(TargetPromotionQuestionnaireDeletedEvent @event);
    }
}