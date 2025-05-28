
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Gene;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.UpdateExpansionProp
{
    public class UpdateExpansionPropHandler : IRequestHandler<UpdateExpansionPropCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<UpdateExpansionPropHandler> _logger;
        private readonly IGeneExpansionPropRepo _expansionPropRepo;
        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;

        public UpdateExpansionPropHandler(IMapper mapper, ILogger<UpdateExpansionPropHandler> logger, IGeneExpansionPropRepo expansionPropRepo, IEventSourcingHandler<GeneAggregate> eventSourcingHandler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _expansionPropRepo = expansionPropRepo ?? throw new ArgumentNullException(nameof(expansionPropRepo));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
        }

        public async Task<Unit> Handle(UpdateExpansionPropCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateExpansionPropHandler {request}", request);

            request.SetUpdateProperties(request.RequestorUserId);
            
            var geneExpansionPropUpdatedEvent = _mapper.Map<GeneExpansionPropUpdatedEvent>(request);
            geneExpansionPropUpdatedEvent.LastModifiedById = request.RequestorUserId;

            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.UpdateExpansionProp(geneExpansionPropUpdatedEvent);
                await _eventSourcingHandler.SaveAsync(aggregate);

                return Unit.Value;
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(GeneAggregate), request.Id);
            }
        }
    }
}