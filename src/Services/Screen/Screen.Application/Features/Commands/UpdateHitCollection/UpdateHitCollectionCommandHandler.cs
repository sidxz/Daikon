using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;
using Screen.Domain.Entities;


namespace Screen.Application.Features.Commands.UpdateHitCollection
{
    public class UpdateHitCollectionCommandHandler : IRequestHandler<UpdateHitCollectionCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<UpdateHitCollectionCommandHandler> _logger;    
        private readonly IHitCollectionRepository _hitCollectionRepository;
        private readonly IEventSourcingHandler<HitCollectionAggregate> _hitCollectionEventSourcingHandler;
        

        public UpdateHitCollectionCommandHandler(ILogger<UpdateHitCollectionCommandHandler> logger, 
            IEventSourcingHandler<HitCollectionAggregate> hitCollectionEventSourcingHandler,
            IHitCollectionRepository hitCollectionRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hitCollectionRepository = hitCollectionRepository ?? throw new ArgumentNullException(nameof(hitCollectionRepository));
            _hitCollectionEventSourcingHandler = hitCollectionEventSourcingHandler ?? throw new ArgumentNullException(nameof(hitCollectionEventSourcingHandler));
           
        }
        public async Task<Unit> Handle(UpdateHitCollectionCommand request, CancellationToken cancellationToken)
        {
            var existingHitCollection = await _hitCollectionRepository.ReadHitCollectionById(request.HitCollectionId);

            if (existingHitCollection.Name != request.Name)
            {
                throw new InvalidOperationException("Name cannot be modified");
            }
            if (existingHitCollection.ScreenId != request.ScreenId)
            {
                throw new InvalidOperationException("ScreenId cannot be modified");
            }


            var updatedHitCollection = _mapper.Map<HitCollection>(request);
            updatedHitCollection.HitCollectionId = request.HitCollectionId;
            updatedHitCollection.Id = request.HitCollectionId;
            updatedHitCollection.Name = existingHitCollection.Name;
            updatedHitCollection.ScreenId = existingHitCollection.ScreenId;
            
            try
            {
                var aggregate = await _hitCollectionEventSourcingHandler.GetByAsyncId(request.HitCollectionId);
                aggregate.UpdateHitCollection(updatedHitCollection, _mapper);
                await _hitCollectionEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(HitCollectionAggregate), request.HitCollectionId);
            }
            return Unit.Value;
        }
    }
}