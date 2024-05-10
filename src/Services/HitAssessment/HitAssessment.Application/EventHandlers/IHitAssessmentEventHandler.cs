
using Daikon.Events.HitAssessment;

namespace HitAssessment.Application.EventHandlers
{
    public interface IHitAssessmentEventHandler
    {
        Task OnEvent(HaCreatedEvent @event);
        Task OnEvent(HaUpdatedEvent @event);
        Task OnEvent(HaRenamedEvent @event);
        Task OnEvent(HaDeletedEvent @event);

        Task OnEvent(HaCompoundEvolutionAddedEvent @event);
        Task OnEvent(HaCompoundEvolutionUpdatedEvent @event);
        Task OnEvent(HaCompoundEvolutionDeletedEvent @event);


    }
}