
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Targets;
using MediatR;
using Microsoft.Extensions.Logging;
using Target.Domain.Aggregates;

namespace Target.Application.Features.Commands.AddToxicology
{
    public class AddToxicologyHandler
        (IMapper mapper, ILogger<AddToxicologyHandler> logger, IEventSourcingHandler<TargetAggregate> eventSourcingHandler)
     : IRequestHandler<AddToxicologyCommand, Unit>
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly ILogger<AddToxicologyHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IEventSourcingHandler<TargetAggregate> _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));

        public async Task<Unit> Handle(AddToxicologyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AddToxicologyCommand {request}", request);

            request.SetCreateProperties(request.RequestorUserId);

            var targetToxicologyAddedEvent = _mapper.Map<TargetToxicologyAddedEvent>(request);
            targetToxicologyAddedEvent.CreatedById = request.RequestorUserId;

            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.AddToxicology(targetToxicologyAddedEvent);
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