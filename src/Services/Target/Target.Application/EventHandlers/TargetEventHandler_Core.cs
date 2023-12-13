

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
        private ILogger<TargetEventHandler> _logger;

        private IMapper _mapper;

        public TargetEventHandler(ITargetRepository targetRepository, ILogger<TargetEventHandler> logger, IMapper mapper)
        {
            _targetRepository = targetRepository;
            _logger = logger;
            _mapper = mapper;
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

        public Task OnEvent(TargetUpdatedEvent @event)
        {
            throw new NotImplementedException();
        }

        public Task OnEvent(TargetDeletedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}