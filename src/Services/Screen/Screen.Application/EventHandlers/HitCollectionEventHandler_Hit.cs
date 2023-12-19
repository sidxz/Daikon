
using CQRS.Core.Exceptions;
using Daikon.Events.Screens;
using Microsoft.Extensions.Logging;


namespace Screen.Application.EventHandlers
{
    public partial class HitCollectionEventHandler : IHitCollectionEventHandler
    {

        public async Task OnEvent(HitAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: HitAddedEvent: {Id}", @event.Id);
            var hit = _mapper.Map<Domain.Entities.Hit>(@event);
            hit.Id = @event.HitId;
            hit.HitCollectionId = @event.HitCollectionId;
            hit.HitId = @event.HitId;
            hit.DateCreated = DateTime.UtcNow;
            hit.IsModified = false;

            try
            {
                await _hitRepository.CreateHit(hit);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "HitAddedEvent Error creating hit", ex);
            }
        }

        public async Task OnEvent(HitUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: HitUpdatedEvent: {Id}", @event.Id);
            var existingHit = await _hitRepository.ReadHitById(@event.HitId);

            if (existingHit == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitUpdatedEvent Error updating hit {@event.HitId}", new Exception("Hit not found"));
            }

            var hit = _mapper.Map<Domain.Entities.Hit>(@event);
            hit.Id = @event.HitId;
            hit.HitCollectionId = @event.HitCollectionId;
            hit.HitId = @event.HitId;
            hit.DateCreated = existingHit.DateCreated;
            hit.IsModified = true;

            try
            {
                await _hitRepository.UpdateHit(hit);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitUpdatedEvent Error updating hit {@event.HitId}", ex);
            }

        }

        public async Task OnEvent(HitDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: HitDeletedEvent: {Id}", @event.Id);

            var existingHit = await _hitRepository.ReadHitById(@event.HitId);
            if (existingHit == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitDeletedEvent Error deleting hit {@event.HitId}", new Exception("Hit not found"));
            }

            try
            {
                await _hitRepository.DeleteHit(@event.HitId);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitDeletedEvent Error deleting hit {@event.HitId}", ex);
            }
        }
    }
}