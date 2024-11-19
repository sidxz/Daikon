
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Gene;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.UpdateCrispriStrain
{
    public class UpdateCrispriStrainCommandHandler : IRequestHandler<UpdateCrispriStrainCommand, Unit>
    {

        private readonly ILogger<UpdateCrispriStrainCommandHandler> _logger;

        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
        private readonly IMapper _mapper;

        public UpdateCrispriStrainCommandHandler(ILogger<UpdateCrispriStrainCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IMapper mapper)
        {
            _logger = logger;
            _eventSourcingHandler = eventSourcingHandler;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateCrispriStrainCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateCrispriStrainCommandHandler {request}", request);
            request.SetUpdateProperties(request.RequestorUserId);

            var geneCrispriStrainUpdatedEvent = _mapper.Map<GeneCrispriStrainUpdatedEvent>(request);
            geneCrispriStrainUpdatedEvent.LastModifiedById = request.RequestorUserId;


            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.UpdateCrispriStrain(geneCrispriStrainUpdatedEvent);
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