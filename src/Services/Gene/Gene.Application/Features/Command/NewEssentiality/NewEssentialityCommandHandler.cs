
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using Gene.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.NewEssentiality
{
    public class NewEssentialityCommandHandler : IRequestHandler<NewEssentialityCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<NewEssentialityCommandHandler> _logger;

        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
        private readonly IGeneRepository _geneRepository;
        private readonly IStrainRepository _strainRepository;

        public NewEssentialityCommandHandler(ILogger<NewEssentialityCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IGeneRepository geneRepository, IStrainRepository strainRepository, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
            _geneRepository = geneRepository ?? throw new ArgumentNullException(nameof(geneRepository));
            _strainRepository = strainRepository ?? throw new ArgumentNullException(nameof(strainRepository));
        }


        public async Task<Unit> Handle(NewEssentialityCommand request, CancellationToken cancellationToken)
        {
            
            var newEssentiality = _mapper.Map<Essentiality>(request);
            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.AddEssentiality(newEssentiality);
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