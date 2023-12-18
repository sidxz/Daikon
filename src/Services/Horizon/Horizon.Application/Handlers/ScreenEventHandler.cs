
using Daikon.Events.Screens;
using Horizon.Application.Contracts.Persistance;
using Horizon.Application.Query.Handlers;
using Horizon.Domain.Screens;
using Microsoft.Extensions.Logging;

namespace Horizon.Application.Handlers
{
    public class ScreenEventHandler : IScreenEventHandler
    {
        private readonly ILogger<ScreenEventHandler> _logger;
        private readonly IGraphRepositoryForScreen _graphRepository;

        public ScreenEventHandler(ILogger<ScreenEventHandler> logger, IGraphRepositoryForScreen graphRepository)
        {
            _logger = logger;
            _graphRepository = graphRepository;
        }

        public async Task OnEvent(ScreenCreatedEvent @event)
        {
            _logger.LogInformation($"Horizon: ScreenCreatedEvent: {@event.Id} {@event.Name}");
            var screen = new Screen
            {
                ScreenId = @event.Id.ToString(),
                StrainId = @event.StrainId.ToString(),

                Name = @event.Name,
                AssociatedTargetsId = @event.AssociatedTargets.Keys.ToList(),
                ScreenType = @event.ScreenType,
                Method = @event.Method,
                Status = @event.Status,
                PrimaryOrgName = @event.PrimaryOrgName,

                DateCreated = DateTime.UtcNow,
                IsModified = false,
                IsDraft = false
            };

            await _graphRepository.AddScreenToGraph(screen);
        }

        public Task OnEvent(ScreenUpdatedEvent @event)
        {
            _logger.LogInformation($"Horizon: ScreenUpdatedEvent: {@event.Id} {@event.Name}");
            throw new NotImplementedException();
        }

        public Task OnEvent(ScreenAssociatedTargetsUpdatedEvent @event)
        {
            throw new NotImplementedException();
        }

        public Task OnEvent(ScreenDeletedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}