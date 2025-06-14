
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Gene;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.UpdateProteinProduction
{
    public class UpdateProteinProductionCommandHandler : IRequestHandler<UpdateProteinProductionCommand, Unit>
    {

        private readonly ILogger<UpdateProteinProductionCommandHandler> _logger;

        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
        private readonly IMapper _mapper;

        public UpdateProteinProductionCommandHandler(ILogger<UpdateProteinProductionCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IMapper mapper)
        {
            _logger = logger;
            _eventSourcingHandler = eventSourcingHandler;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateProteinProductionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateProteinProductionCommandHandler {request}", request);

            request.SetUpdateProperties(request.RequestorUserId);

            var geneProteinProductionUpdatedEvent = _mapper.Map<GeneProteinProductionUpdatedEvent>(request);
            geneProteinProductionUpdatedEvent.LastModifiedById = request.RequestorUserId;

            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);

                aggregate.UpdateProteinProduction(geneProteinProductionUpdatedEvent);
                await _eventSourcingHandler.SaveAsync(aggregate);

                return Unit.Value;
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(StrainAggregate), request.Id);
            }

        }
    }
}