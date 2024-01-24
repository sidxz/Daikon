
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.Events.Screens;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;

namespace Screen.Application.EventHandlers
{
    public partial class ScreenEventHandler : IScreenEventHandler
    {

        private readonly IScreenRepository _screenRepository;
        private readonly IScreenRunRepository _screenRunRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ScreenEventHandler> _logger;

        public ScreenEventHandler(IScreenRepository screenRepository, IScreenRunRepository screenRunRepository,
                                    IMapper mapper, ILogger<ScreenEventHandler> logger)
        {
            _screenRepository = screenRepository;
            _screenRunRepository = screenRunRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task OnEvent(ScreenCreatedEvent @event)
        {
            _logger.LogInformation("OnEvent: ScreenCreatedEvent: {Id}", @event.Id);
            var screen = _mapper.Map<Domain.Entities.Screen>(@event);
            screen.Id = @event.Id;
            screen.DateCreated = DateTime.UtcNow;
            screen.IsModified = false;

            try
            {
                await _screenRepository.CreateScreen(screen);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "ScreenCreatedEvent Error creating screen", ex);
            }
        }

        public async Task OnEvent(ScreenUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: ScreenUpdatedEvent: {Id}", @event.Id);
            var existingScreen = await _screenRepository.ReadScreenById(@event.Id);

            if (existingScreen == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"ScreenUpdatedEvent Error updating screen {@event.Id}", new Exception("Screen not found"));
            }

            var screen = _mapper.Map<Domain.Entities.Screen>(@event);
            screen.DateCreated = existingScreen.DateCreated;
            screen.DateModified = DateTime.UtcNow;
            screen.IsModified = true;

            try
            {
                await _screenRepository.UpdateScreen(screen);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"ScreenUpdatedEvent Error updating screen {@event.Id}", ex);
            }
        }

        public async Task OnEvent(ScreenDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: ScreenDeletedEvent: {Id}", @event.Id);
            try
            {
                await _screenRepository.DeleteScreen(@event.Id);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"ScreenDeletedEvent Error deleting screen {@event.Id}", ex);
            }

        }

        public async Task OnEvent(ScreenRenamedEvent @event)
        {
            _logger.LogInformation("OnEvent: ScreenRenamedEvent: {Id}", @event.Id);
            var screen = await _screenRepository.ReadScreenById(@event.Id);

            if (screen == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"ScreenRenamedEvent Error renaming screen {@event.Id}", new Exception("Screen not found"));
            }

            screen.Name = @event.Name;
            screen.DateModified = DateTime.UtcNow;
            screen.IsModified = true;

            try
            {
                await _screenRepository.UpdateScreen(screen);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"ScreenRenamedEvent Error renaming screen {@event.Id}", ex);
            }
        }


        public async Task OnEvent(ScreenAssociatedTargetsUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: ScreenAssociatedTargetsUpdatedEvent: {Id}", @event.Id);
            var screen = await _screenRepository.ReadScreenById(@event.Id);

            if (screen == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"ScreenAssociatedTargetsUpdatedEvent Error updating screen {@event.Id}", new Exception("Screen not found"));
            }

            screen.AssociatedTargets = @event.AssociatedTargets;
            screen.DateModified = DateTime.UtcNow;
            screen.IsModified = true;

            try
            {
                await _screenRepository.UpdateScreen(screen);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"ScreenAssociatedTargetsUpdatedEvent Error updating screen {@event.Id}", ex);
            }
        }

    }
}