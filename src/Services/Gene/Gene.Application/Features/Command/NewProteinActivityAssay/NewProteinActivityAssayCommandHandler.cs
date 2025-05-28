
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Gene;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using Gene.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.NewProteinActivityAssay
{
    public class NewProteinActivityAssayCommandHandler : IRequestHandler<NewProteinActivityAssayCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<NewProteinActivityAssayCommandHandler> _logger;
        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;


        public NewProteinActivityAssayCommandHandler(ILogger<NewProteinActivityAssayCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IGeneRepository geneRepository, IStrainRepository strainRepository, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
        }


        public async Task<Unit> Handle(NewProteinActivityAssayCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("NewProteinActivityAssayCommandHandler {request}", request);
            request.SetCreateProperties(request.RequestorUserId);

            var geneProteinActivityAssayAddedEvent = _mapper.Map<GeneProteinActivityAssayAddedEvent>(request);
            geneProteinActivityAssayAddedEvent.CreatedById = request.RequestorUserId;

            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.AddProteinActivityAssay(geneProteinActivityAssayAddedEvent);
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