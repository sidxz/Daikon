
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Gene;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.AddExpansionProp
{
    public class AddExpansionPropHandler : IRequestHandler<AddExpansionPropCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<AddExpansionPropHandler> _logger;
        private readonly IGeneExpansionPropRepo _expansionPropRepo;
        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;

        public AddExpansionPropHandler(IMapper mapper, ILogger<AddExpansionPropHandler> logger, IGeneExpansionPropRepo expansionPropRepo, IEventSourcingHandler<GeneAggregate> eventSourcingHandler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _expansionPropRepo = expansionPropRepo ?? throw new ArgumentNullException(nameof(expansionPropRepo));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
        }

        public async Task<Unit> Handle(AddExpansionPropCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AddExpansionPropCommandHandler {request}", request);
            
            request.SetCreateProperties(request.RequestorUserId);
            
            var geneExpansionPropAddedEvent = _mapper.Map<GeneExpansionPropAddedEvent>(request);

            geneExpansionPropAddedEvent.CreatedById = request.RequestorUserId;


            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.AddExpansionProp(geneExpansionPropAddedEvent);
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