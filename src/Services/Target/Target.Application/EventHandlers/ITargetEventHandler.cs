
using Daikon.Events.Targets;

namespace Target.Application.EventHandlers
{
    public interface ITargetEventHandler
    {
        Task OnEvent(TargetCreatedEvent @event);
        Task OnEvent(TargetUpdatedEvent @event);
        Task OnEvent(TargetDeletedEvent @event);
    }
}