
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Gene;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using Gene.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.NewCrispriStrain
{
    public class NewCrispriStrainCommandHandler : IRequestHandler<NewCrispriStrainCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<NewCrispriStrainCommandHandler> _logger;
        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
        

        public NewCrispriStrainCommandHandler(ILogger<NewCrispriStrainCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IGeneRepository geneRepository, IStrainRepository strainRepository, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
        }


        public async Task<Unit> Handle(NewCrispriStrainCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling NewCrispriStrainCommand: {request}");
            request.SetCreateProperties(request.RequestorUserId);

            var newCrispriCreatedEvent = _mapper.Map<GeneCrispriStrainAddedEvent>(request);
            newCrispriCreatedEvent.CreatedById = request.RequestorUserId;
            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.AddCrispriStrain(newCrispriCreatedEvent);
                await _eventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(GeneAggregate), request.Id);
            }
            return Unit.Value;

        }
    }

}