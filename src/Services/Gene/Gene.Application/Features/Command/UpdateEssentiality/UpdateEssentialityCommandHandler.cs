
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Domain.Aggregates;
using Gene.Domain.Entities;
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
            var updateEssentiality = _mapper.Map<Essentiality>(request);


            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                
                aggregate.UpdateEssentiality(updateEssentiality);
                await _eventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(StrainAggregate), request.Id);
            }
            return Unit.Value;
        }
    }
}