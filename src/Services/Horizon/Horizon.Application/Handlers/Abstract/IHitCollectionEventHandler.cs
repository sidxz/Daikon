

using Daikon.Events.Screens;

namespace Horizon.Application.Handlers
{
    public interface IHitCollectionEventHandler
    {
        Task OnEvent(HitCollectionCreatedEvent @event);
        Task OnEvent(HitCollectionUpdatedEvent @event);
        Task OnEvent(HitCollectionDeletedEvent @event);
        Task OnEvent(HitCollectionRenamedEvent @event);
        Task OnEvent(HitCollectionAssociatedScreenUpdatedEvent @event);
        Task OnEvent(HitAddedEvent @event);
        Task OnEvent(HitUpdatedEvent @event);
        Task OnEvent(HitDeletedEvent @event);
        Task OnEvent(HitMoleculeUpdatedEvent @event);

    }
}