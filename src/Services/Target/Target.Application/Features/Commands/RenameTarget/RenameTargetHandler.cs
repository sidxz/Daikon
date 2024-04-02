
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Targets;
using MediatR;
using Microsoft.Extensions.Logging;
using Target.Application.Contracts.Persistence;
using Target.Domain.Aggregates;

namespace Target.Application.Features.Commands.RenameTarget
{
    public class RenameTargetHandler : IRequestHandler<RenameTargetCommand, Unit>
    {
        private readonly ILogger<RenameTargetHandler> _logger;
        private readonly IMapper _mapper;

        private readonly IEventSourcingHandler<TargetAggregate> _eventSourcingHandler;
        private readonly ITargetRepository _targetRepository;

        public RenameTargetHandler(ILogger<RenameTargetHandler> logger, IMapper mapper, IEventSourcingHandler<TargetAggregate> eventSourcingHandler, ITargetRepository targetRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _eventSourcingHandler = eventSourcingHandler;
            _targetRepository = targetRepository;
        }

        public async Task<Unit> Handle(RenameTargetCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling RenameTargetCommand");
            var existingTarget = await _targetRepository.ReadTargetById(request.Id) ?? throw new InvalidOperationException("Target not found");
            if (existingTarget.Name == request.Name)
            {
                _logger.LogWarning("Name is the same as the existing name");
                throw new InvalidOperationException("Name is the same as the existing name");
            }

            // check if name is available
            var checkTargetName = await _targetRepository.ReadTargetByName(request.Name);
            if (checkTargetName != null)
            {
                _logger.LogWarning("Name is already taken");
                throw new DuplicateEntityRequestException(nameof(TargetAggregate), request.Name);
            }

            var targetRenamedEvent = _mapper.Map<TargetRenamedEvent>(request);

            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.RenameTarget(targetRenamedEvent);
                await _eventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(TargetAggregate), request.Id);
            }


            return Unit.Value;
        }
    }
}