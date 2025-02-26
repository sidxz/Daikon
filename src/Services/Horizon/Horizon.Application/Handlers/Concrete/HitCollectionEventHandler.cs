
using Daikon.Events.Screens;
using Horizon.Application.Contracts.Persistence;
using Horizon.Domain.Screens;
using Microsoft.Extensions.Logging;

namespace Horizon.Application.Handlers
{
    public class HitCollectionEventHandler : IHitCollectionEventHandler
    {
        private readonly ILogger<HitCollectionEventHandler> _logger;
        private readonly IGraphRepositoryForHitCollection _graphRepository;

        public HitCollectionEventHandler(ILogger<HitCollectionEventHandler> logger, IGraphRepositoryForHitCollection graphRepository)
        {
            _logger = logger;
            _graphRepository = graphRepository;
        }

        public async Task OnEvent(HitCollectionCreatedEvent @event)
        {
            _logger.LogInformation($"Horizon: HitCollectionCreatedEvent: {@event.Id} {@event.Name}");
            var hitCollection = new HitCollection
            {
                UniId = @event.Id.ToString(),
                HitCollectionId = @event.Id.ToString(),
                ScreenId = @event.ScreenId.ToString(),
                Name = @event.Name,
                HitCollectionType = @event.HitCollectionType,

                DateCreated = @event.DateCreated,
                IsModified = @event.IsModified,
                IsDraft = @event.IsDraft

            };

            await _graphRepository.AddHitCollection(hitCollection);
        }

        public async Task OnEvent(HitCollectionUpdatedEvent @event)
        {
            _logger.LogInformation($"Horizon: HitCollectionUpdatedEvent: {@event.Id} {@event.Name}");
            var hitCollection = new HitCollection
            {
                UniId = @event.Id.ToString(),
                HitCollectionId = @event.Id.ToString(),
                ScreenId = @event.ScreenId.ToString(),
                Name = @event.Name,
                HitCollectionType = @event.HitCollectionType,
                DateCreated = @event.DateCreated,
                DateModified = @event.DateModified,
                IsModified = @event.IsModified,
                IsDraft = @event.IsDraft


            };
            await _graphRepository.UpdateHitCollection(hitCollection);
        }

        public Task OnEvent(HitCollectionDeletedEvent @event)
        {
            _logger.LogInformation($"Horizon: HitCollectionDeletedEvent: {@event.Id}");
            return _graphRepository.DeleteHitCollection(@event.Id.ToString());
        }

        public Task OnEvent(HitCollectionRenamedEvent @event)
        {
            _logger.LogInformation($"Horizon: HitCollectionRenamedEvent: {@event.Id} {@event.Name}");
            return _graphRepository.RenameHitCollection(@event.Id.ToString(), @event.Name);
        }

        public async Task OnEvent(HitAddedEvent @event)
        {
            _logger.LogInformation($"Horizon: HitAddedEvent: {@event.Id} {@event.HitId}");
            var hit = new Hit
            {
                UniId = @event.HitId.ToString(),
                HitId = @event.HitId.ToString(),
                HitCollectionId = @event.Id.ToString(),
                Library = @event.Library ?? "",
                RequestedSMILES = @event.RequestedSMILES ?? "",
                MoleculeId = @event.MoleculeId.ToString(),
                MoleculeRegistrationId = @event.MoleculeRegistrationId.ToString() ?? "",

                DateCreated = @event?.DateCreated ?? DateTime.Now,
                IsModified = @event?.IsModified ?? true,
                IsDraft = @event?.IsDraft ?? false
            };
            await _graphRepository.AddHit(hit);
        }

        public async Task OnEvent(HitUpdatedEvent @event)
        {
            _logger.LogInformation($"Horizon: HitUpdatedEvent: {@event.Id} {@event.HitId}");
            var hit = new Hit
            {
                UniId = @event.HitId.ToString(),
                HitId = @event.HitId.ToString(),
                HitCollectionId = @event.Id.ToString(),
                Library = @event.Library ?? "",

                DateModified = @event?.DateModified ?? DateTime.Now,
                IsModified = @event?.IsModified ?? true,
                IsDraft = @event?.IsDraft ?? false
            };
            await _graphRepository.UpdateHit(hit);
        }

        public Task OnEvent(HitDeletedEvent @event)
        {
            _logger.LogInformation($"Horizon: HitDeletedEvent: {@event.Id}");
            return _graphRepository.DeleteHit(@event.HitId.ToString());
        }

        public Task OnEvent(HitCollectionAssociatedScreenUpdatedEvent @event)
        {
            _logger.LogInformation($"Horizon: HitCollectionAssociatedScreenUpdatedEvent: {@event.Id} {@event.ScreenId}");
            return _graphRepository.UpdateAssociatedScreenOfHitCollection(new HitCollection
            {
                HitCollectionId = @event.Id.ToString(),
                ScreenId = @event.ScreenId.ToString(),

                DateModified = @event?.DateModified ?? DateTime.Now,
                IsModified = @event?.IsModified ?? true,
                IsDraft = @event?.IsDraft ?? false
            });
        }
    }
}