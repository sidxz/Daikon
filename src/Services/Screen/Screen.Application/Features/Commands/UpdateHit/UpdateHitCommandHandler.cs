using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;
using Screen.Domain.Entities;


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
           
           if (request.HitCollectionId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(request.HitCollectionId));
            }
            
            var updatedHit = _mapper.Map<Hit>(request);
            updatedHit.HitCollectionId = request.Id;
            
            try
            {
                var aggregate = await _hitCollectionEventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.UpdateHit(updatedHit);
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