using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.Events.Screens;

namespace Screen.Application.EventHandlers
{
    public interface IHitCollectionEventHandler
    {
        Task OnEvent(HitCollectionCreatedEvent @event);
        Task OnEvent(HitCollectionUpdatedEvent @event);
        Task OnEvent(HitCollectionDeletedEvent @event);
        Task OnEvent(HitAddedEvent @event);
        Task OnEvent(HitUpdatedEvent @event);
        Task OnEvent(HitDeletedEvent @event);
    }
}