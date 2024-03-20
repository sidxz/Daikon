

using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.Events.Targets;
using Microsoft.Extensions.Logging;
using Target.Application.Contracts.Persistence;

namespace Target.Application.EventHandlers
{
    public partial class TargetEventHandler : ITargetEventHandler
    {

        private readonly ITargetRepository _targetRepository;

        private readonly IPQResponseRepository _pqResponseRepository;
        private ILogger<TargetEventHandler> _logger;

        private IMapper _mapper;

        public TargetEventHandler(ITargetRepository targetRepository, IPQResponseRepository pqResponseRepository,
            ILogger<TargetEventHandler> logger, IMapper mapper)
        {
            _targetRepository = targetRepository;
            _logger = logger;
            _mapper = mapper;
            _pqResponseRepository = pqResponseRepository;
        }

        public async Task OnEvent(TargetCreatedEvent @event)
        {
            _logger.LogInformation("OnEvent: TargetCreatedEvent: {Id}", @event.Id);
            var target = _mapper.Map<Domain.Entities.Target>(@event);
            target.Id = @event.Id;
            target.DateCreated = DateTime.UtcNow;
            target.IsModified = false;

            try
            {
                await _targetRepository.CreateTarget(target);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "TargetCreatedEvent Error creating target", ex);
            }
        }

        public async Task OnEvent(TargetUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: TargetUpdatedEvent: {Id}", @event.Id);
            var existingTarget = await _targetRepository.ReadTargetById(@event.Id);

            if (existingTarget == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"TargetUpdatedEvent Error updating target {@event.Id}", new Exception("Target not found"));
            }

            var target = _mapper.Map<Domain.Entities.Target>(@event);
            target.DateCreated = existingTarget.DateCreated;
            target.IsModified = true;

            try
            {
                await _targetRepository.UpdateTarget(target);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"TargetUpdatedEvent Error updating target {@event.Id}", ex);
            }
        }


        public async Task OnEvent(TargetAssociatedGenesUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: TargetAssociatedGenesUpdatedEvent: {Id}", @event.Id);
            var target = await _targetRepository.ReadTargetById(@event.Id);

            if (target == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"TargetUpdatedEvent Error updating target {@event.Id}", new Exception("Target not found"));
            }

            target.AssociatedGenes = @event.AssociatedGenes;
            target.IsModified = true;

            try
            {
                await _targetRepository.UpdateTarget(target);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"TargetUpdatedEvent Error updating target {@event.Id}", ex);
            }
        }



        public async Task OnEvent(TargetDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: TargetUpdatedEvent: {Id}", @event.Id);

            var existingTarget = await _targetRepository.ReadTargetById(@event.Id);
            if (existingTarget == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"TargetDeletedEvent Error deleting target {@event.Id}", new Exception("Target not found"));
            }


            try
            {
                await _targetRepository.DeleteTarget(existingTarget);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"TargetDeletedEvent Error deleting target {@event.Id}", ex);
            }
        }
    }
}