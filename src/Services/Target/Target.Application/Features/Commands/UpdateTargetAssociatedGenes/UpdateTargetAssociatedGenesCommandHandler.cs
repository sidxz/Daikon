
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Target.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
using Daikon.Events.Targets;

namespace Target.Application.Features.Command.UpdateTargetAssociatedGenes
{
    public class UpdateTargetAssociatedGenesCommandHandler : IRequestHandler<UpdateTargetAssociatedGenesCommand, Unit>
    {

        private readonly ILogger<UpdateTargetAssociatedGenesCommandHandler> _logger;
        private readonly IMapper _mapper;

        private readonly IEventSourcingHandler<TargetAggregate> _eventSourcingHandler;


        public UpdateTargetAssociatedGenesCommandHandler(ILogger<UpdateTargetAssociatedGenesCommandHandler> logger, IEventSourcingHandler<TargetAggregate> eventSourcingHandler,
                                        IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _eventSourcingHandler = eventSourcingHandler;
        }

        public async Task<Unit> Handle(UpdateTargetAssociatedGenesCommand request, CancellationToken cancellationToken)
        {
            request.SetUpdateProperties(request.RequestorUserId);
            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);

                var targetUpdatedEvent = _mapper.Map<TargetAssociatedGenesUpdatedEvent>(request);

                aggregate.UpdateTargetAssociatedGenes(targetUpdatedEvent);

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