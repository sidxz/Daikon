
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Targets;
using MediatR;
using Microsoft.Extensions.Logging;
using Target.Domain.Aggregates;

namespace Target.Application.Features.Commands.UpdateToxicology
{
    public class UpdateToxicologyHandler
        (IMapper mapper, ILogger<UpdateToxicologyHandler> logger, IEventSourcingHandler<TargetAggregate> eventSourcingHandler)
        : IRequestHandler<UpdateToxicologyCommand, Unit>
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly ILogger<UpdateToxicologyHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IEventSourcingHandler<TargetAggregate> _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));

        public async Task<Unit> Handle(UpdateToxicologyCommand request, CancellationToken cancellationToken)
        {
            request.SetUpdateProperties(request.RequestorUserId);
            _logger.LogInformation("UpdateToxicologyHandler {request}", request);

            var targetToxicologyUpdatedEvent = _mapper.Map<TargetToxicologyUpdatedEvent>(request);
            targetToxicologyUpdatedEvent.LastModifiedById = request.RequestorUserId;


            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);

                aggregate.UpdateToxicology(targetToxicologyUpdatedEvent);
                await _eventSourcingHandler.SaveAsync(aggregate);

                return Unit.Value;
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(TargetAggregate), request.Id);
            }
        }
    }
}