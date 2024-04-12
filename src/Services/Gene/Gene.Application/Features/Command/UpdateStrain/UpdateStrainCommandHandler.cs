
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Strains;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Gene.Application.Features.Command.UpdateStrain
{
    public class UpdateStrainCommandHandler : IRequestHandler<UpdateStrainCommand, Unit>
    {

        private readonly ILogger<UpdateStrainCommandHandler> _logger;
        private readonly IEventSourcingHandler<StrainAggregate> _eventSourcingHandler;
        private readonly IMapper _mapper;

        public UpdateStrainCommandHandler(ILogger<UpdateStrainCommandHandler> logger,
                 IEventSourcingHandler<StrainAggregate> eventSourcingHandler,
                 IMapper mapper
                 )
        {
            _logger = logger;
            _eventSourcingHandler = eventSourcingHandler;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateStrainCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateStrainCommandHandler {request}", request);
            request.DateModified = DateTime.UtcNow;
            request.IsModified = true;

            var strainUpdatedEvent = _mapper.Map<StrainUpdatedEvent>(request);
            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);

                aggregate.UpdateStrain(strainUpdatedEvent);
                await _eventSourcingHandler.SaveAsync(aggregate);

                return Unit.Value;
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(StrainAggregate), request.Id);
            }

        }
    }
}