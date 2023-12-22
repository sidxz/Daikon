
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
            try
            {
                _logger.LogInformation($"Horizon: Start-> ScreenCreatedEvent: {@event.Id} {@event.Name}");
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
                };
                _logger.LogInformation($"Horizon: Send to repo ->ScreenCreatedEvent: {@event.Id} {@event.Name} {screen.DateCreated}");

                await _graphRepository.AddScreen(screen);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding screen");
                throw;
            }
        }

        public async Task OnEvent(ScreenUpdatedEvent @event)
        {
            _logger.LogInformation($"Horizon: ScreenUpdatedEvent: {@event.Id} {@event.Name}");
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
                IsModified = true,
            };

            await _graphRepository.UpdateScreen(screen);
        }

        public async Task OnEvent(ScreenAssociatedTargetsUpdatedEvent @event)
        {
            _logger.LogInformation($"Horizon: ScreenAssociatedTargetsUpdatedEvent: {@event.Id} {@event.Name}");
            var screen = new Screen
            {
                ScreenId = @event.Id.ToString(),
                Name = @event.Name,
                AssociatedTargetsId = @event.AssociatedTargets.Keys.ToList(),
                DateCreated = DateTime.UtcNow,
                IsModified = true,
            };

            await _graphRepository.UpdateScreen(screen);
        }

        public Task OnEvent(ScreenDeletedEvent @event)
        {
            _logger.LogInformation($"Horizon: ScreenDeletedEvent: {@event.Id}");
            return _graphRepository.DeleteScreen(@event.Id.ToString());
        }

        public Task OnEvent(ScreenRenamedEvent @event)
        {
            _logger.LogInformation($"Horizon: ScreenRenamedEvent: {@event.Id} {@event.Name}");
            return _graphRepository.RenameScreen(@event.Id.ToString(), @event.Name);
        }
    }
}