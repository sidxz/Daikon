
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.Events.Screens;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;

namespace Screen.Application.EventHandlers
{
    public partial class HitCollectionEventHandler : IHitCollectionEventHandler
    {
        private readonly IHitCollectionRepository _hitCollectionRepository;
        private readonly IHitRepository _hitRepository;
        private readonly IScreenRepository _screenRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<HitCollectionEventHandler> _logger;

        public HitCollectionEventHandler(IHitCollectionRepository hitCollectionRepository, IHitRepository hitRepository, 
        IMapper mapper, ILogger<HitCollectionEventHandler> logger, IScreenRepository screenRepository)
        {
            _hitCollectionRepository = hitCollectionRepository;
            _hitRepository = hitRepository;
            _mapper = mapper;
            _logger = logger;
            _screenRepository = screenRepository;
        }

        public async Task OnEvent(HitCollectionCreatedEvent @event)
        {
            _logger.LogInformation("OnEvent: HitCollectionCreatedEvent: {Id}", @event.Id);
            var hitCollection = _mapper.Map<Domain.Entities.HitCollection>(@event);
            hitCollection.Id = @event.Id;
            hitCollection.DateCreated = DateTime.UtcNow;
            hitCollection.IsModified = false;

            var screen = await _screenRepository.ReadScreenById(@event.ScreenId);
            screen.DeepLastUpdated = DateTime.UtcNow;

            try
            {
                await _hitCollectionRepository.CreateHitCollection(hitCollection);
                await _screenRepository.UpdateScreen(screen);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "HitCollectionCreatedEvent Error creating hit collection", ex);
            }
        }

        public async Task OnEvent(HitCollectionUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: HitCollectionUpdatedEvent: {Id}", @event.Id);
            var existingHitCollection = await _hitCollectionRepository.ReadHitCollectionById(@event.Id);

            if (existingHitCollection == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitCollectionUpdatedEvent Error updating hit collection {@event.Id}", new Exception("Hit collection not found"));
            }
            var hitCollection = _mapper.Map<Domain.Entities.HitCollection>(@event);
            hitCollection.DateCreated = existingHitCollection.DateCreated;
            hitCollection.IsModified = true;

            var screen = await _screenRepository.ReadScreenById(@event.ScreenId);
            screen.DeepLastUpdated = DateTime.UtcNow;

            try
            {
                await _hitCollectionRepository.UpdateHitCollection(hitCollection);
                await _screenRepository.UpdateScreen(screen);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitCollectionUpdatedEvent Error updating hit collection {@event.Id}", ex);
            }
        }

        public async Task OnEvent(HitCollectionDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: HitCollectionDeletedEvent: {Id}", @event.Id);
            var existingHitCollection = await _hitCollectionRepository.ReadHitCollectionById(@event.Id);

            if (existingHitCollection == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitCollectionDeletedEvent Error deleting hit collection {@event.Id}", new Exception("Hit collection not found"));
            }

            // Delete all hits in the hit collection
            try
            {
                await _hitRepository.DeleteHitsByHitCollectionId(@existingHitCollection.Id);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitCollectionDeletedEvent Error deleting hits in hit collection {@event.Id}", ex);
            }

            // Delete the hit collection
            try
            {
                await _hitCollectionRepository.DeleteHitCollection(existingHitCollection.Id);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitCollectionDeletedEvent Error deleting hit collection {@event.Id}", ex);
            }

        }

        public async Task OnEvent(HitCollectionRenamedEvent @event)
        {
            _logger.LogInformation("OnEvent: HitCollectionRenamedEvent: {Id}", @event.Id);
            var hitCollection = await _hitCollectionRepository.ReadHitCollectionById(@event.Id);


            if (hitCollection == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitCollectionRenamedEvent Error renaming hit collection {@event.Id}", new Exception("Hit collection not found"));
            }
            hitCollection.Name = @event.Name;
            hitCollection.DateModified = DateTime.UtcNow;
            hitCollection.IsModified = true;

            try
            {
                await _hitCollectionRepository.UpdateHitCollection(hitCollection);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitCollectionRenamedEvent Error renaming hit collection {@event.Id}", ex);
            }
        }


        public async Task OnEvent(HitCollectionAssociatedScreenUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: HitCollectionAssociatedScreenUpdatedEvent: {Id}", @event.Id);
            var hitCollection = await _hitCollectionRepository.ReadHitCollectionById(@event.Id);

            if (hitCollection == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitCollectionAssociatedScreenUpdatedEvent Error updating hit collection {@event.Id}", new Exception("Hit collection not found"));
            }
            hitCollection.ScreenId = @event.ScreenId;
            hitCollection.DateModified = DateTime.UtcNow;
            hitCollection.IsModified = true;

            try
            {
                await _hitCollectionRepository.UpdateHitCollection(hitCollection);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitCollectionAssociatedScreenUpdatedEvent Error updating hit collection {@event.Id}", ex);
            }
        }

    }
}