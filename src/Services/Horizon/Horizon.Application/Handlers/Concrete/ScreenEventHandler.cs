using Daikon.Events.Screens;
using Horizon.Application.Contracts.Persistence;
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
                    UniId = @event.Id.ToString(),
                    ScreenId = @event.Id.ToString(),
                    StrainId = @event.StrainId?.ToString() ?? "",

                    Name = @event.Name,
                    AssociatedTargetsId = @event.AssociatedTargets?.Keys.ToList() ?? [],
                    ScreenType = @event?.ScreenType ?? "",
                    Method = @event?.Method ?? "",
                    Status = @event?.Status ?? "",
                    PrimaryOrgId = @event?.PrimaryOrgId?.ToString() ?? "",

                    DateCreated = @event?.DateCreated ?? DateTime.Now,
                    IsModified = @event?.IsModified ?? false,
                    IsDraft = @event?.IsDraft ?? false
                };

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
            try
            {
                _logger.LogInformation($"Horizon: ScreenUpdatedEvent: {@event.Id} {@event.Name}");
                var screen = new Screen
                {
                    UniId = @event.Id.ToString(),
                    ScreenId = @event.Id.ToString(),
                    StrainId = @event.StrainId?.ToString() ?? "",

                    Name = @event.Name,
                    AssociatedTargetsId = @event.AssociatedTargets?.Keys.ToList() ?? [],
                    ScreenType = @event?.ScreenType ?? "",
                    Method = @event?.Method ?? "",
                    Status = @event?.Status ?? "",
                    PrimaryOrgId = @event?.PrimaryOrgId?.ToString() ?? "",

                    DateModified = @event?.DateModified ?? DateTime.Now,
                    IsModified = @event?.IsModified ?? true,
                    IsDraft = @event?.IsDraft ?? false
                };

                await _graphRepository.UpdateScreen(screen);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating screen");
                throw;
            }
        }

        public async Task OnEvent(ScreenAssociatedTargetsUpdatedEvent @event)
        {
            try
            {
                _logger.LogInformation($"Horizon: ScreenAssociatedTargetsUpdatedEvent: {@event.Id} {@event.Name}");
                var screen = new Screen
                {
                    UniId = @event.Id.ToString(),
                    ScreenId = @event.Id.ToString(),
                    Name = @event.Name,
                    AssociatedTargetsId = @event.AssociatedTargets?.Keys.ToList() ?? [],

                    DateModified = @event?.DateModified ?? DateTime.Now,
                    IsModified = @event?.IsModified ?? true,
                    IsDraft = @event?.IsDraft ?? false
                };

                await _graphRepository.UpdateAssociatedTargetsOfScreen(screen);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating associated targets for screen");
                throw;
            }
        }

        public async Task OnEvent(ScreenDeletedEvent @event)
        {
            try
            {
                _logger.LogInformation($"Horizon: ScreenDeletedEvent: {@event.Id}");
                await _graphRepository.DeleteScreen(@event.Id.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting screen");
                throw;
            }
        }

        public async Task OnEvent(ScreenRenamedEvent @event)
        {
            try
            {
                _logger.LogInformation($"Horizon: ScreenRenamedEvent: {@event.Id} {@event.Name}");
                await _graphRepository.RenameScreen(@event.Id.ToString(), @event.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renaming screen");
                throw;
            }
        }
    }
}