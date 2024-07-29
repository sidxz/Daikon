
using Daikon.Events.Targets;

namespace Target.Application.EventHandlers
{
    public interface ITargetEventHandler
    {
        Task OnEvent(TargetCreatedEvent @event);
        Task OnEvent(TargetUpdatedEvent @event);
        Task OnEvent(TargetDeletedEvent @event);
        Task OnEvent(TargetRenamedEvent @event);
        Task OnEvent(TargetAssociatedGenesUpdatedEvent @event);
        Task OnEvent(TargetPromotionQuestionnaireSubmittedEvent @event);
        Task OnEvent(TargetPromotionQuestionnaireUpdatedEvent @event);
        Task OnEvent(TargetPromotionQuestionnaireDeletedEvent @event);

        Task OnEvent(TargetToxicologyAddedEvent @event);
        Task OnEvent(TargetToxicologyUpdatedEvent @event);
        Task OnEvent(TargetToxicologyDeletedEvent @event);
    }
}