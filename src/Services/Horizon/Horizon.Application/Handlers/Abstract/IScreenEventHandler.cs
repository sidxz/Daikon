
using Daikon.Events.Screens;

namespace Horizon.Application.Query.Handlers
{
    public interface IScreenEventHandler
    {
        Task OnEvent(ScreenCreatedEvent @event);
        Task OnEvent(ScreenUpdatedEvent @event);
        Task OnEvent(ScreenAssociatedTargetsUpdatedEvent @event);
        Task OnEvent(ScreenDeletedEvent @event);
        Task OnEvent(ScreenRenamedEvent @event);
        
    }
}