using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;


namespace Screen.Application.Features.Commands.UpdateHitCollectionAssociatedScreen
{
    public class UpdateHitCollectionAssociatedScreenCommandHandler : IRequestHandler<UpdateHitCollectionAssociatedScreenCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<UpdateHitCollectionAssociatedScreenCommandHandler> _logger;    
        private readonly IHitCollectionRepository _hitCollectionRepository;
        private readonly IEventSourcingHandler<HitCollectionAggregate> _hitCollectionEventSourcingHandler;
        

        public UpdateHitCollectionAssociatedScreenCommandHandler(ILogger<UpdateHitCollectionAssociatedScreenCommandHandler> logger, 
            IEventSourcingHandler<HitCollectionAggregate> hitCollectionEventSourcingHandler,
            IHitCollectionRepository hitCollectionRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hitCollectionRepository = hitCollectionRepository ?? throw new ArgumentNullException(nameof(hitCollectionRepository));
            _hitCollectionEventSourcingHandler = hitCollectionEventSourcingHandler ?? throw new ArgumentNullException(nameof(hitCollectionEventSourcingHandler));
           
        }
        public async Task<Unit> Handle(UpdateHitCollectionAssociatedScreenCommand request, CancellationToken cancellationToken)
        {
            var existingHitCollection = await _hitCollectionRepository.ReadHitCollectionById(request.Id);

            if (existingHitCollection == null)
            {
                throw new ResourceNotFoundException(nameof(HitCollectionAggregate), request.Id);
            }

            if (existingHitCollection.ScreenId == request.ScreenId)
            {
                throw new InvalidOperationException("ScreenId is the same as the existing ScreenId");
            }

            
            try
            {
                var aggregate = await _hitCollectionEventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.UpdateHitCollectionAssociatedScreen(request.ScreenId);
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