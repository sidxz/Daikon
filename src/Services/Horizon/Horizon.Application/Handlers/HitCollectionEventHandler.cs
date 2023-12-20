using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.Events.Screens;
using Horizon.Application.Contracts.Persistance;
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
                HitCollectionId = @event.Id.ToString(),
                ScreenId = @event.ScreenId.ToString(),
                Name = @event.Name,
                HitCollectionType = @event.HitCollectionType,
                

                DateCreated = DateTime.UtcNow,
                IsModified = false,

            };

            await _graphRepository.AddHitCollectionToGraph(hitCollection);
        }

        public async Task OnEvent(HitCollectionUpdatedEvent @event)
        {
            _logger.LogInformation($"Horizon: HitCollectionUpdatedEvent: {@event.Id} {@event.Name}");
            var hitCollection = new HitCollection
            {
                HitCollectionId = @event.Id.ToString(),
                ScreenId = @event.ScreenId.ToString(),
                Name = @event.Name,
                HitCollectionType = @event.HitCollectionType,
                
                IsModified = true,

            };
            await _graphRepository.UpdateHitCollectionOfGraph(hitCollection);
        }

        public Task OnEvent(HitCollectionDeletedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}