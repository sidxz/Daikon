using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Screens;
using Daikon.Shared.Constants.AppScreen;
using MediatR;
using Microsoft.Extensions.Logging;
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

            request.SetUpdateProperties(request.RequestorUserId);

            try
            {
                if (request.VoteToAdd != null)
                {
                    if (request.VoteToAdd.Item1 != request.RequestorUserId.ToString())
                    {
                        throw new ArgumentException("User cannot cast a vote for another user");
                    }

                    // check if the casted vote is valid
                    if (request.VoteToAdd.Item2 != VotingValue.Positive.ToString() && request.VoteToAdd.Item2 != VotingValue.Negative.ToString() && request.VoteToAdd.Item2 != VotingValue.Neutral.ToString())
                    {
                        throw new ArgumentException("Vote value must be Positive, Negative, or Neutral");
                    }
                }

                var hitUpdatedEvent = _mapper.Map<HitUpdatedEvent>(request);
               

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