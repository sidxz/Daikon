
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Gene;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.DeleteExpansionProp
{
    public class DeleteExpansionPropHandler : IRequestHandler<DeleteExpansionPropCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteExpansionPropHandler> _logger;
        private readonly IGeneExpansionPropRepo _expansionPropRepo;
        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;

        public DeleteExpansionPropHandler(IMapper mapper, ILogger<DeleteExpansionPropHandler> logger, IGeneExpansionPropRepo expansionPropRepo, IEventSourcingHandler<GeneAggregate> eventSourcingHandler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _expansionPropRepo = expansionPropRepo ?? throw new ArgumentNullException(nameof(expansionPropRepo));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
        }
        public async Task<Unit> Handle(DeleteExpansionPropCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.SetUpdateProperties(request.RequestorUserId);
                var expansionPropDeletedEvent = _mapper.Map<GeneExpansionPropDeletedEvent>(request);

                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.DeleteExpansionProp(expansionPropDeletedEvent);
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