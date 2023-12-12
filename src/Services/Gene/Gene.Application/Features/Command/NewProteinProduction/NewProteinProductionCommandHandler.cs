
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using Gene.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.NewProteinProduction
{
    public class NewProteinProductionCommandHandler : IRequestHandler<NewProteinProductionCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<NewProteinProductionCommandHandler> _logger;
        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
        

        public NewProteinProductionCommandHandler(ILogger<NewProteinProductionCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IGeneRepository geneRepository, IStrainRepository strainRepository, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
        }


        public async Task<Unit> Handle(NewProteinProductionCommand request, CancellationToken cancellationToken)
        {
            
            var newProteinProduction = _mapper.Map<ProteinProduction>(request);
            
            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.AddProteinProduction(newProteinProduction);
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