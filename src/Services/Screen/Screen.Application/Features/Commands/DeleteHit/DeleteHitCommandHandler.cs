using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;
using Screen.Domain.Entities;


namespace Screen.Application.Features.Commands.DeleteHit
{
    public class DeleteHitCommandHandler : IRequestHandler<DeleteHitCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<DeleteHitCommandHandler> _logger;
        private readonly IHitRepository _hitRepository;

        private readonly IEventSourcingHandler<HitCollectionAggregate> _hitCollectionEventSourcingHandler;


        public DeleteHitCommandHandler(ILogger<DeleteHitCommandHandler> logger,
            IEventSourcingHandler<HitCollectionAggregate> hitCollectionEventSourcingHandler,
            IHitRepository hitRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hitRepository = hitRepository ?? throw new ArgumentNullException(nameof(hitRepository));
            _hitCollectionEventSourcingHandler = hitCollectionEventSourcingHandler ?? throw new ArgumentNullException(nameof(hitCollectionEventSourcingHandler));

        }

        public async Task<Unit> Handle(DeleteHitCommand request, CancellationToken cancellationToken)
        {

            if (request.Id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(request.Id));
            }

            request.HitCollectionId = request.Id;

            try
            {
                var aggregate = await _hitCollectionEventSourcingHandler.GetByAsyncId(request.HitCollectionId);
                aggregate.DeleteHit(request.HitId);
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