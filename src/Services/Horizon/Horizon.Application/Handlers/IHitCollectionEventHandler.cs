

using Daikon.Events.Screens;

namespace Horizon.Application.Handlers
{
    public interface IHitCollectionEventHandler
    {
        Task OnEvent(HitCollectionCreatedEvent @event);
        Task OnEvent(HitCollectionUpdatedEvent @event);
        Task OnEvent(HitCollectionDeletedEvent @event);
    }
}