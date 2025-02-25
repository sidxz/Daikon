using CQRS.Core.Exceptions;
using Daikon.Events.Screens;
using Daikon.Shared.Constants.AppScreen;
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

            var hitCollection = await _hitCollectionRepository.ReadHitCollectionById(@event.Id);
            var screen = await _screenRepository.ReadScreenById(hitCollection.ScreenId);
            screen.DeepLastUpdated = DateTime.UtcNow;

            try
            {
                await _hitRepository.CreateHit(hit);
                await _screenRepository.UpdateScreen(screen);
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
            hit.IsModified = true;

            hit.Voters = existingHit.Voters ?? [];
            hit.Positive = existingHit.Positive ?? 0;
            hit.Negative = existingHit.Negative ?? 0;
            hit.Neutral = existingHit.Neutral ?? 0;


            // Voting Update
            if (@event.VoteToAdd != null)
            {
                if (@event.VoteToAdd.Item1 != @event.RequestorUserId.ToString())
                {
                    throw new ArgumentException("User cannot cast a vote for another user");
                }

                // check if the casted vote is valid
                if (@event.VoteToAdd.Item2 != VotingValue.Positive.ToString() && @event.VoteToAdd.Item2 != VotingValue.Negative.ToString() && @event.VoteToAdd.Item2 != VotingValue.Neutral.ToString())
                {
                    throw new ArgumentException("Vote value must be Positive, Negative, or Neutral");
                }

                if (@event.VoteToAdd.Item2 == VotingValue.Positive.ToString())
                {
                    hit.Voters[@event.VoteToAdd.Item1] = VotingValue.Positive;
                    hit.Positive++;
                }
                else if (@event.VoteToAdd.Item2 == VotingValue.Negative.ToString())
                {
                    hit.Voters[@event.VoteToAdd.Item1] = VotingValue.Negative;
                    hit.Negative++;
                }
                else if (@event.VoteToAdd.Item2 == VotingValue.Neutral.ToString())
                {
                    hit.Voters[@event.VoteToAdd.Item1] = VotingValue.Neutral;
                    hit.Neutral++;
                }
            }

            var hitCollection = await _hitCollectionRepository.ReadHitCollectionById(@event.Id);
            var screen = await _screenRepository.ReadScreenById(hitCollection.ScreenId);
            screen.DeepLastUpdated = DateTime.UtcNow;

            try
            {
                await _hitRepository.UpdateHit(hit);
                await _screenRepository.UpdateScreen(screen);
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

            var hitCollection = await _hitCollectionRepository.ReadHitCollectionById(@event.Id);
            var screen = await _screenRepository.ReadScreenById(hitCollection.ScreenId);
            screen.DeepLastUpdated = DateTime.UtcNow;

            try
            {
                await _hitRepository.DeleteHit(@event.HitId);
                await _screenRepository.UpdateScreen(screen);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while deleting hit {@event.HitId} in HitDeletedEvent", ex);
            }
        }
    }
}