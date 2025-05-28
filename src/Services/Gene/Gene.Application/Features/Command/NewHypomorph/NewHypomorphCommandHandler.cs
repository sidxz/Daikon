
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Gene;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using Gene.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.NewHypomorph
{
    public class NewHypomorphCommandHandler : IRequestHandler<NewHypomorphCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<NewHypomorphCommandHandler> _logger;
        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;


        public NewHypomorphCommandHandler(ILogger<NewHypomorphCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IGeneRepository geneRepository, IStrainRepository strainRepository, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
        }


        public async Task<Unit> Handle(NewHypomorphCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling NewHypomorphCommand: {request}");
            request.SetCreateProperties(request.RequestorUserId);
            
            var geneHypomorphAddedEvent = _mapper.Map<GeneHypomorphAddedEvent>(request);
            geneHypomorphAddedEvent.CreatedById = request.RequestorUserId;

            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.AddHypomorph(geneHypomorphAddedEvent);
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