
using Daikon.Events.HitAssessment;

namespace HitAssessment.Application.EventHandlers
{
    public interface IHitAssessmentEventHandler
    {
        Task OnEvent(HaCreatedEvent @event);
        Task OnEvent(HaUpdatedEvent @event);
        Task OnEvent(HaDeletedEvent @event);


    }
}