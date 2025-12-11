
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Targets;
using MediatR;
using Microsoft.Extensions.Logging;
using Target.Domain.Aggregates;
using Target.Domain.Services;

namespace Target.Application.Features.Commands.RenameTarget
{
    public class RenameTargetHandler : IRequestHandler<RenameTargetCommand, Unit>
    {
        private readonly ILogger<RenameTargetHandler> _logger;
        private readonly IMapper _mapper;

        private readonly IEventSourcingHandler<TargetAggregate> _eventSourcingHandler;
        private readonly ITargetUniquenessChecker _targetUniquenessChecker;

        public RenameTargetHandler(ILogger<RenameTargetHandler> logger, IMapper mapper, IEventSourcingHandler<TargetAggregate> eventSourcingHandler, ITargetUniquenessChecker targetUniquenessChecker)
        {
            _logger = logger;
            _mapper = mapper;
            _eventSourcingHandler = eventSourcingHandler;
            _targetUniquenessChecker = targetUniquenessChecker;
        }

        public async Task<Unit> Handle(RenameTargetCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling RenameTargetCommand");
            request.SetUpdateProperties(request.RequestorUserId);
            await _targetUniquenessChecker.EnsureTargetNameIsUniqueAsync(request.StrainId, request.Name, request.Id);

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