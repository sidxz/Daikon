
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Gene;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.UpdateGene
{
    public class UpdateGeneCommandHandler : IRequestHandler<UpdateGeneCommand, Unit>
    {

        private readonly ILogger<UpdateGeneCommandHandler> _logger;
        private readonly IMapper _mapper;

        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
        private readonly IGeneRepository _geneRepository;
        private readonly IStrainRepository _strainRepository;


        public UpdateGeneCommandHandler(ILogger<UpdateGeneCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler,
                                        IGeneRepository geneRepository, IStrainRepository strainRepository, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _eventSourcingHandler = eventSourcingHandler;
            _geneRepository = geneRepository;
            _strainRepository = strainRepository;
        }

        public async Task<Unit> Handle(UpdateGeneCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling UpdateGeneCommand: {request}");

            
           request.SetUpdateProperties(request.RequestorUserId);

           var geneUpdatedEvent = _mapper.Map<GeneUpdatedEvent>(request);
           geneUpdatedEvent.LastModifiedById = request.RequestorUserId;

            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.UpdateGene(geneUpdatedEvent);
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