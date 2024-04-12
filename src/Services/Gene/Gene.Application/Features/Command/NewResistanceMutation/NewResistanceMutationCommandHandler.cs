
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Gene;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.NewResistanceMutation
{
    public class NewResistanceMutationCommandHandler : IRequestHandler<NewResistanceMutationCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<NewResistanceMutationCommandHandler> _logger;
        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;


        public NewResistanceMutationCommandHandler(ILogger<NewResistanceMutationCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IGeneRepository geneRepository, IStrainRepository strainRepository, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
        }


        public async Task<Unit> Handle(NewResistanceMutationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("NewResistanceMutationCommandHandler {request}", request);

            request.DateCreated = DateTime.UtcNow;
            request.IsModified = false;
            var geneResistanceMutationAddedEvent = _mapper.Map<GeneResistanceMutationAddedEvent>(request);

            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.AddResistanceMutation(geneResistanceMutationAddedEvent);
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