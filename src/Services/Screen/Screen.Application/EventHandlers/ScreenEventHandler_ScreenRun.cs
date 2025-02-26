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
            _logger.LogInformation("Processing ScreenRunAddedEvent - ScreenId: {ScreenId}, ScreenRunId: {ScreenRunId}",
                @event.Id, @event.ScreenRunId);
            try
            {
                var screenRun = _mapper.Map<ScreenRun>(@event);
                var screen = await _screenRepository.ReadScreenById(@event.Id)
                             ?? throw new EventHandlerException(nameof(ScreenEventHandler),
                                $"Screen not found for ID {@event.Id}", new Exception("EntityNotFound"));


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


                await _screenRepository.UpdateScreen(screen);
                await _screenRunRepository.CreateScreenRun(screenRun);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(ScreenEventHandler),
                    "Error while creating screen run", ex);
            }
        }

        public async Task OnEvent(ScreenRunUpdatedEvent @event)
        {
            _logger.LogInformation("Processing ScreenRunUpdatedEvent - ScreenId: {ScreenId}, ScreenRunId: {ScreenRunId}",
                @event.Id, @event.ScreenRunId);

            try
            {
                var existingScreenRun = await _screenRunRepository.ReadScreenRunById(@event.ScreenRunId)
                                        ?? throw new EventHandlerException(nameof(ScreenEventHandler),
                                           $"ScreenRun not found for ID {@event.ScreenRunId}", new Exception("EntityNotFound"));

                var screen = await _screenRepository.ReadScreenById(@event.Id)
                             ?? throw new EventHandlerException(nameof(ScreenEventHandler),
                                $"Screen not found for ID {@event.Id}", new Exception("EntityNotFound"));

                screen.DeepLastUpdated = DateTime.UtcNow;

                var screenRun = _mapper.Map<ScreenRun>(existingScreenRun);

                _mapper.Map(@event, screenRun);

                screenRun.Id = @event.ScreenRunId;
                screenRun.ScreenId = @event.Id;

                // Preserve the original creation date and creator
                screenRun.CreatedById = existingScreenRun.CreatedById;
                screenRun.DateCreated = existingScreenRun.DateCreated;


                await _screenRunRepository.UpdateScreenRun(screenRun);
                await _screenRepository.UpdateScreen(screen);

            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(ScreenEventHandler),
                                    "Error while updating screen run", ex);
            }
        }

        public async Task OnEvent(ScreenRunDeletedEvent @event)
        {
            _logger.LogInformation("Processing ScreenRunDeletedEvent - ScreenRunId: {ScreenRunId}", @event.ScreenRunId);

            try
            {
                var existingScreenRun = await _screenRunRepository.ReadScreenRunById(@event.ScreenRunId)
                                        ?? throw new EventHandlerException(nameof(ScreenEventHandler),
                                           $"ScreenRun not found for ID {@event.ScreenRunId}", new Exception("EntityNotFound"));

                var screen = await _screenRepository.ReadScreenById(existingScreenRun.ScreenId)
                             ?? throw new EventHandlerException(nameof(ScreenEventHandler),
                                $"Screen not found for ID {existingScreenRun.ScreenId}", new Exception("EntityNotFound"));

                screen.DeepLastUpdated = DateTime.UtcNow;

                await _screenRunRepository.DeleteScreenRun(@event.ScreenRunId);
                await _screenRepository.UpdateScreen(screen);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(ScreenEventHandler),
                    "Error while deleting screen run", ex);
            }
        }

    }
}