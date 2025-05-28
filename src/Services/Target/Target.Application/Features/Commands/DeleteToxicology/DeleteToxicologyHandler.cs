using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Targets;
using MediatR;
using Microsoft.Extensions.Logging;
using Target.Domain.Aggregates;

namespace Target.Application.Features.Commands.DeleteToxicology
{
    public class DeleteToxicologyHandler
     (IMapper mapper, ILogger<DeleteToxicologyCommand> logger, IEventSourcingHandler<TargetAggregate> eventSourcingHandler)
     : IRequestHandler<DeleteToxicologyCommand, Unit>
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly ILogger<DeleteToxicologyCommand> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IEventSourcingHandler<TargetAggregate> _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));

        public async Task<Unit> Handle(DeleteToxicologyCommand request, CancellationToken cancellationToken)
        {
            request.SetUpdateProperties(request.RequestorUserId);
            _logger.LogInformation("DeleteToxicologyCommand {request}", request);
            var targetToxicologyDeletedEvent = _mapper.Map<TargetToxicologyDeletedEvent>(request);
            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);

                aggregate.DeleteToxicology(targetToxicologyDeletedEvent);
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