
using Daikon.Events.Screens;

namespace Screen.Application.EventHandlers
{
    public interface IScreenEventHandler
    {
        Task OnEvent(ScreenCreatedEvent @event);
        Task OnEvent(ScreenUpdatedEvent @event);
        Task OnEvent(ScreenDeletedEvent @event);
        
    }
}