using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Screens;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;


namespace Screen.Application.Features.Commands.UpdateHit
{
    public class UpdateHitCommandHandler : IRequestHandler<UpdateHitCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<UpdateHitCommandHandler> _logger;
        private readonly IHitRepository _hitRepository;

        private readonly IEventSourcingHandler<HitCollectionAggregate> _hitCollectionEventSourcingHandler;
  

        public UpdateHitCommandHandler(ILogger<UpdateHitCommandHandler> logger, 
            IEventSourcingHandler<HitCollectionAggregate> hitCollectionEventSourcingHandler,
            IHitRepository hitRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hitRepository = hitRepository ?? throw new ArgumentNullException(nameof(hitRepository));
            _hitCollectionEventSourcingHandler = hitCollectionEventSourcingHandler ?? throw new ArgumentNullException(nameof(hitCollectionEventSourcingHandler));
          
        }

        public async Task<Unit> Handle(UpdateHitCommand request, CancellationToken cancellationToken)
        {
           
            try
            {
                var hitUpdatedEvent = _mapper.Map<HitUpdatedEvent>(request);
                _logger.LogInformation($"Updating Hit: {request.Id}");
                _logger.LogInformation(request.ToJson());
                _logger.LogInformation(request.Voters.ToJson());
                _logger.LogInformation(hitUpdatedEvent.ToJson());
                _logger.LogInformation(hitUpdatedEvent.Voters.ToJson());


                var aggregate = await _hitCollectionEventSourcingHandler.GetByAsyncId(request.Id);

                aggregate.UpdateHit(hitUpdatedEvent);

                await _hitCollectionEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(HitCollectionAggregate), request.Id);
            }
            return Unit.Value;
        }
    }
}