using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;
using Screen.Domain.Entities;

namespace Screen.Application.Features.Commands.DeleteHitCollection
{
    public class DeleteHitCollectionCommandHandler : IRequestHandler<DeleteHitCollectionCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<DeleteHitCollectionCommandHandler> _logger;
        private readonly IHitCollectionRepository _hitCollectionRepository;

        private readonly IEventSourcingHandler<HitCollectionAggregate> _hitCollectionEventSourcingHandler;

        public DeleteHitCollectionCommandHandler(ILogger<DeleteHitCollectionCommandHandler> logger,
            IEventSourcingHandler<HitCollectionAggregate> hitCollectionEventSourcingHandler,
            IHitCollectionRepository hitCollectionRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hitCollectionRepository = hitCollectionRepository ?? throw new ArgumentNullException(nameof(hitCollectionRepository));
            _hitCollectionEventSourcingHandler = hitCollectionEventSourcingHandler ?? throw new ArgumentNullException(nameof(hitCollectionEventSourcingHandler));

        }

        public async Task<Unit> Handle(DeleteHitCollectionCommand request, CancellationToken cancellationToken)
        {

            if (request.Id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(request.Id));
            }

            request.HitCollectionId = request.Id;

            try
            {
                var aggregate = await _hitCollectionEventSourcingHandler.GetByAsyncId(request.HitCollectionId);
                aggregate.DeleteHitCollection(request.HitCollectionId);
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