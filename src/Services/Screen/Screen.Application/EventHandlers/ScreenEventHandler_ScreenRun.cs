
using CQRS.Core.Exceptions;
using Daikon.Events.Screens;
using Microsoft.Extensions.Logging;

namespace Screen.Application.EventHandlers
{
    public partial class ScreenEventHandler : IScreenEventHandler
    {

        public async Task OnEvent(ScreenRunAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: ScreenRunAddedEvent: {Id}", @event.Id);
            var screenRun = _mapper.Map<Domain.Entities.ScreenRun>(@event);
            screenRun.Id = @event.ScreenRunId;
            screenRun.ScreenId = @event.ScreenId;
            screenRun.ScreenRunId = @event.ScreenRunId;
            screenRun.DateCreated = DateTime.UtcNow;
            screenRun.IsModified = false;

            try
            {
                await _screenRunRepository.CreateScreenRun(screenRun);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "ScreenRunAddedEvent Error creating screen run", ex);
            }
        }

        public async Task OnEvent(ScreenRunUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: ScreenRunUpdatedEvent: {Id}", @event.Id);
            var existingScreenRun = await _screenRunRepository.ReadScreenRunById(@event.ScreenRunId);

            if (existingScreenRun == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"ScreenRunUpdatedEvent Error updating screen run {@event.ScreenRunId}", new Exception("Screen run not found"));
            }

            var screenRun = _mapper.Map<Domain.Entities.ScreenRun>(@event);
            screenRun.Id = @event.ScreenRunId;
            screenRun.ScreenId = @event.ScreenId;
            screenRun.ScreenRunId = @event.ScreenRunId;

            screenRun.DateCreated = existingScreenRun.DateCreated;
            screenRun.IsModified = true;

            try
            {
                await _screenRunRepository.UpdateScreenRun(screenRun);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"ScreenRunUpdatedEvent Error updating screen run {@event.ScreenRunId}", ex);
            }
        }

        public async Task OnEvent(ScreenRunDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: ScreenRunDeletedEvent: {Id}", @event.Id);
            var existingScreenRun = await _screenRunRepository.ReadScreenRunById(@event.ScreenRunId);

            if (existingScreenRun == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"ScreenRunDeletedEvent Error deleting screen run {@event.ScreenRunId}", new Exception("Screen run not found"));
            }

            try
            {
                await _screenRunRepository.DeleteScreenRun(@event.ScreenRunId);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"ScreenRunDeletedEvent Error deleting screen run {@event.ScreenRunId}", ex);
            }
        }

    }
}