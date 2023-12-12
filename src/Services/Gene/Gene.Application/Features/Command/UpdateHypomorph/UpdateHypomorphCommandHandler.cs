
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Domain.Aggregates;
using Gene.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.UpdateHypomorph
{
    public class UpdateHypomorphCommandHandler : IRequestHandler<UpdateHypomorphCommand, Unit>
    {

        private readonly ILogger<UpdateHypomorphCommandHandler> _logger;

        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
        private readonly IMapper _mapper;

        public UpdateHypomorphCommandHandler(ILogger<UpdateHypomorphCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IMapper mapper)
        {
            _logger = logger;
            _eventSourcingHandler = eventSourcingHandler;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateHypomorphCommand request, CancellationToken cancellationToken)
        {
            var updateHypomorph = _mapper.Map<Hypomorph>(request);


            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                
                aggregate.UpdateHypomorph(updateHypomorph);
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