using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.Events.Screens;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using System;
using System.Threading.Tasks;

namespace Screen.Application.EventHandlers
{
    public partial class ScreenEventHandler : IScreenEventHandler
    {
        private readonly IScreenRepository _screenRepository;
        private readonly IScreenRunRepository _screenRunRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ScreenEventHandler> _logger;

        public ScreenEventHandler(
            IScreenRepository screenRepository,
            IScreenRunRepository screenRunRepository,
            IMapper mapper,
            ILogger<ScreenEventHandler> logger)
        {
            _screenRepository = screenRepository;
            _screenRunRepository = screenRunRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task OnEvent(ScreenCreatedEvent @event)
        {
            _logger.LogInformation("Processing ScreenCreatedEvent - ScreenId: {ScreenId}", @event.Id);

            try
            {
                var screen = _mapper.Map<Domain.Entities.Screen>(@event);
                screen.Id = @event.Id;
                screen.DateCreated = DateTime.UtcNow;
                screen.IsModified = false;
                screen.DeepLastUpdated = DateTime.UtcNow;

                await _screenRepository.CreateScreen(screen);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(ScreenEventHandler), "Error creating screen", ex);
            }
        }

        public async Task OnEvent(ScreenUpdatedEvent @event)
        {
            _logger.LogInformation("Processing ScreenUpdatedEvent - ScreenId: {ScreenId}", @event.Id);

            try
            {
                var screen = await _screenRepository.ReadScreenById(@event.Id)
                             ?? throw new EventHandlerException(nameof(ScreenEventHandler), $"Screen not found for ID {@event.Id}", new Exception("EntityNotFound"));

                // Update relevant fields only
                _mapper.Map(@event, screen);
                screen.DateModified = DateTime.UtcNow;
                screen.IsModified = true;
                screen.DeepLastUpdated = DateTime.UtcNow;

                await _screenRepository.UpdateScreen(screen);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(ScreenEventHandler), "Error updating screen", ex);
            }
        }

        public async Task OnEvent(ScreenDeletedEvent @event)
        {
            _logger.LogInformation("Processing ScreenDeletedEvent - ScreenId: {ScreenId}", @event.Id);

            try
            {
                await _screenRepository.DeleteScreen(@event.Id);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(ScreenEventHandler), "Error deleting screen", ex);
            }
        }

        public async Task OnEvent(ScreenRenamedEvent @event)
        {
            _logger.LogInformation("Processing ScreenRenamedEvent - ScreenId: {ScreenId}", @event.Id);

            try
            {
                var screen = await _screenRepository.ReadScreenById(@event.Id)
                             ?? throw new EventHandlerException(nameof(ScreenEventHandler), $"Screen not found for ID {@event.Id}", new Exception("EntityNotFound"));

                screen.Name = @event.Name;
                screen.DateModified = DateTime.UtcNow;
                screen.IsModified = true;
                screen.DeepLastUpdated = DateTime.UtcNow;

                await _screenRepository.UpdateScreen(screen);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(ScreenEventHandler), "Error renaming screen", ex);
            }
        }

        public async Task OnEvent(ScreenAssociatedTargetsUpdatedEvent @event)
        {
            _logger.LogInformation("Processing ScreenAssociatedTargetsUpdatedEvent - ScreenId: {ScreenId}", @event.Id);

            try
            {
                var screen = await _screenRepository.ReadScreenById(@event.Id)
                             ?? throw new EventHandlerException(nameof(ScreenEventHandler), $"Screen not found for ID {@event.Id}", new Exception("EntityNotFound"));

                screen.AssociatedTargets = @event.AssociatedTargets;
                screen.DateModified = DateTime.UtcNow;
                screen.IsModified = true;
                screen.DeepLastUpdated = DateTime.UtcNow;

                await _screenRepository.UpdateScreen(screen);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(ScreenEventHandler), "Error updating associated targets", ex);
            }
        }
    }
}
