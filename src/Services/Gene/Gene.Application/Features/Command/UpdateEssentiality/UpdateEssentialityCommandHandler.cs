
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Gene;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.UpdateEssentiality
{
    public class UpdateEssentialityCommandHandler : IRequestHandler<UpdateEssentialityCommand, Unit>
    {

        private readonly ILogger<UpdateEssentialityCommandHandler> _logger;

        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
        private readonly IMapper _mapper;

        public UpdateEssentialityCommandHandler(ILogger<UpdateEssentialityCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IMapper mapper)
        {
            _logger = logger;
            _eventSourcingHandler = eventSourcingHandler;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateEssentialityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateEssentialityCommandHandler {request}", request);

            request.DateModified = DateTime.UtcNow;
            request.IsModified = true;

            var geneEssentialityUpdatedEvent = _mapper.Map<GeneEssentialityUpdatedEvent>(request);
            geneEssentialityUpdatedEvent.LastModifiedById = request.RequestorUserId;

            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                
                aggregate.UpdateEssentiality(geneEssentialityUpdatedEvent);
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