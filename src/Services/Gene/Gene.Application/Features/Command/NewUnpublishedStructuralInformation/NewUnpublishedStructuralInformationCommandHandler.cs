
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Gene;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.NewUnpublishedStructuralInformation
{
    public class NewUnpublishedStructuralInformationCommandHandler : IRequestHandler<NewUnpublishedStructuralInformationCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<NewUnpublishedStructuralInformationCommandHandler> _logger;
        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;


        public NewUnpublishedStructuralInformationCommandHandler(ILogger<NewUnpublishedStructuralInformationCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IGeneRepository geneRepository, IStrainRepository strainRepository, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
        }


        public async Task<Unit> Handle(NewUnpublishedStructuralInformationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("NewUnpublishedStructuralInformationCommandHandler {request}", request);

            request.SetCreateProperties(request.RequestorUserId);

            var geneUnpublishedStructuralInformationAddedEvent = _mapper.Map<GeneUnpublishedStructuralInformationAddedEvent>(request);
            geneUnpublishedStructuralInformationAddedEvent.CreatedById = request.RequestorUserId;

            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.AddUnpublishedStructuralInformation(geneUnpublishedStructuralInformationAddedEvent);
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