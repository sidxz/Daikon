
using Daikon.Events.Screens;

namespace Screen.Application.EventHandlers
{
    public interface IScreenEventHandler
    {
        Task OnEvent(ScreenCreatedEvent @event);
        Task OnEvent(ScreenUpdatedEvent @event);
        Task OnEvent(ScreenDeletedEvent @event);
        Task OnEvent(ScreenAssociatedTargetsUpdatedEvent @event);
        Task OnEvent(ScreenRenamedEvent @event);
        Task OnEvent(ScreenRunAddedEvent @event);
        Task OnEvent(ScreenRunUpdatedEvent @event);
        Task OnEvent(ScreenRunDeletedEvent @event);


    }
}