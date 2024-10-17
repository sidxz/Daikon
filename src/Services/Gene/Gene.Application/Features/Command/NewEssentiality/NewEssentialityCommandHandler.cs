
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Gene;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.NewEssentiality
{
    public class NewEssentialityCommandHandler : IRequestHandler<NewEssentialityCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<NewEssentialityCommandHandler> _logger;
        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;


        public NewEssentialityCommandHandler(ILogger<NewEssentialityCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IGeneRepository geneRepository, IStrainRepository strainRepository, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
        }


        public async Task<Unit> Handle(NewEssentialityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("NewEssentialityCommandHandler {request}", request);
            request.SetCreateProperties(request.RequestorUserId);

            var geneEssentialityAddedEvent = _mapper.Map<GeneEssentialityAddedEvent>(request);
            geneEssentialityAddedEvent.CreatedById = request.RequestorUserId;

            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.AddEssentiality(geneEssentialityAddedEvent);
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