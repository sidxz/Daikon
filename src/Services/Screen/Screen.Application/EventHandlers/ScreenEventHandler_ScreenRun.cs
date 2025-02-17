using CQRS.Core.Exceptions;
using Daikon.Events.Screens;
using Microsoft.Extensions.Logging;
using Screen.Domain.Entities;

namespace Screen.Application.EventHandlers
{
    public partial class ScreenEventHandler : IScreenEventHandler
    {

        public async Task OnEvent(ScreenRunAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: ScreenRunAddedEvent: ScreenId {Id}, ScreenRunId {ScreenRunId}", @event.Id, @event.ScreenRunId);

            var screenRun = _mapper.Map<Domain.Entities.ScreenRun>(@event);
            var screen = await _screenRepository.ReadScreenById(@event.Id);
            screen.DeepLastUpdated = DateTime.UtcNow;

            // Override screenRun.Id to be the same as screenRun.ScreenRunId 
            // as @event.Id refers to ScreenId (Aggregate Id) which is auto-mapped by the mapper
            // In MongoDb, we want to use ScreenRunId as the Id of the entity
            screenRun.Id = @event.ScreenRunId;

            // Set the ScreenId to be the same as the Aggregate Id to maintain a 
            // relationship between Screen and ScreenRun
            screenRun.ScreenId = @event.Id;

            screenRun.DateCreated = DateTime.UtcNow;
            screenRun.IsModified = false;

            try
            {
                await _screenRunRepository.CreateScreenRun(screenRun);
                await _screenRepository.UpdateScreen(screen);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "Error occurred while creating screen run for ScreenRunAddedEvent", ex);
            }
        }

        public async Task OnEvent(ScreenRunUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: ScreenRunUpdatedEvent: ScreenId {Id}, ScreenRunId {ScreenRunId}", @event.Id, @event.ScreenRunId);
            //_logger.LogInformation("OnEvent: ScreenRunUpdatedEventJSON: {event}", System.Text.Json.JsonSerializer.Serialize(@event));

            try
            {
                var existingScreenRun = await _screenRunRepository.ReadScreenRunById(@event.ScreenRunId);

                var screenRun = _mapper.Map<ScreenRun>(existingScreenRun);
                //_logger.LogInformation("OnEvent: ScreenRunUpdatedEvent: ExistingScreenRunJSON: {existingScreenRun}", System.Text.Json.JsonSerializer.Serialize(existingScreenRun));
                
                
                _mapper.Map(@event, screenRun);

                screenRun.Id = @event.ScreenRunId;
                screenRun.ScreenId = @event.Id;

                // Preserve the original creation date and creator
                screenRun.CreatedById = existingScreenRun.CreatedById;
                screenRun.DateCreated = existingScreenRun.DateCreated;

                //_logger.LogInformation("OnEvent: ScreenRunUpdatedEvent: ScreenRunToUpdateJSON: {screenRun}", System.Text.Json.JsonSerializer.Serialize(screenRun));

                var screen = await _screenRepository.ReadScreenById(@event.Id);
                screen.DeepLastUpdated = DateTime.UtcNow;

                await _screenRunRepository.UpdateScreenRun(screenRun);
                await _screenRepository.UpdateScreen(screen);
                
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while updating screen run {@event.ScreenRunId} for ScreenRunUpdatedEvent", ex);
            }
        }

        public async Task OnEvent(ScreenRunDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: ScreenRunDeletedEvent: {Id}", @event.Id);
            var existingScreenRun = await _screenRunRepository.ReadScreenRunById(@event.ScreenRunId);

            if (existingScreenRun == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while deleting screen run {@event.ScreenRunId} for ScreenRunDeletedEvent", new Exception("Screen run not found"));
            }

            var screen = await _screenRepository.ReadScreenById(existingScreenRun.ScreenId);
            screen.DeepLastUpdated = DateTime.UtcNow;

            try
            {
                await _screenRunRepository.DeleteScreenRun(@event.ScreenRunId);
                await _screenRepository.UpdateScreen(screen);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while deleting screen run {@event.ScreenRunId} for ScreenRunDeletedEvent", ex);
            }
        }

    }
}