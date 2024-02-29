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

            // @override hit.Id to be the same as hit.hitCollectionID 
            // as @event.Id refers to HitCollectionId (Aggregate Id) which is auto mapped by mapper
            // In MongoDb, we want to use hitID as the Id of the entity
            hit.Id = @event.HitId;

            // Set the HitCollectionId to be the same as the Aggregate Id to maintain a 
            // relationship between Hit and HitCollection
            hit.HitCollectionId = @event.Id;
            hit.DateCreated = DateTime.UtcNow;
            hit.IsModified = false;

            try
            {
                await _hitRepository.CreateHit(hit);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "Error occurred while creating hit in HitAddedEvent", ex);
            }
        }

        public async Task OnEvent(HitUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: HitUpdatedEvent: {Id}", @event.Id);
            var existingHit = await _hitRepository.ReadHitById(@event.HitId);

            if (existingHit == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while updating hit {@event.HitId} in HitUpdatedEvent", new Exception("Hit not found"));
            }

            var hit = _mapper.Map<Domain.Entities.Hit>(@event);
            hit.Id = @event.HitId;
            hit.HitCollectionId = @event.Id;
            
            hit.DateCreated = existingHit.DateCreated;
            hit.RequestedSMILES = existingHit.RequestedSMILES;
            hit.IsStructureDisclosed = existingHit.IsStructureDisclosed;
            hit.MoleculeId = existingHit.MoleculeId;
            hit.MoleculeRegistrationId = existingHit.MoleculeRegistrationId;
            hit.IsVotingAllowed = existingHit.IsVotingAllowed;
            hit.Positive = existingHit.Positive;
            hit.Neutral = existingHit.Neutral;
            hit.Negative = existingHit.Negative;
            hit.IsModified = true;

            try
            {
                await _hitRepository.UpdateHit(hit);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while updating hit {@event.HitId} in HitUpdatedEvent", ex);
            }

        }

        public async Task OnEvent(HitDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: HitDeletedEvent: {Id}", @event.Id);

            var existingHit = await _hitRepository.ReadHitById(@event.HitId);
            if (existingHit == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while deleting hit {@event.HitId} in HitDeletedEvent", new Exception("Hit not found"));
            }

            try
            {
                await _hitRepository.DeleteHit(@event.HitId);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while deleting hit {@event.HitId} in HitDeletedEvent", ex);
            }
        }
    }
}