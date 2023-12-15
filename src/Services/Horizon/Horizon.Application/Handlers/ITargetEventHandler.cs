
using Daikon.Events.Targets;

namespace Horizon.Application.Query.Handlers
{
    public interface ITargetEventHandler
    {
        Task OnEvent(TargetCreatedEvent @event);
        Task OnEvent(TargetUpdatedEvent @event);
        Task OnEvent(TargetAssociatedGenesUpdatedEvent @event);
        Task OnEvent(TargetDeletedEvent @event);
        
    }
}