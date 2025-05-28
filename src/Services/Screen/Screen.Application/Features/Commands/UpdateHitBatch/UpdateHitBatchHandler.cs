using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Screens;
using MediatR;
using Microsoft.Extensions.Logging;
using Daikon.Shared.VM.Screen;using Screen.Domain.Aggregates;
using Screen.Domain.Entities;
using CQRS.Core.Extensions;
using Screen.Application.Features.Commands.UpdateHit;

namespace Screen.Application.Features.Commands.UpdateHitBatch
{
    /*
     * Handles the bulk update of Hit entities within a HitCollection.
     * Applies validation, maps commands to events, updates aggregates,
     * and persists changes using event sourcing.
     */
    public class UpdateHitBatchHandler : IRequestHandler<UpdateHitBatchCommand, List<HitVM>>
    {
        private const int BatchSize = 500;

        private readonly IMapper _mapper;
        private readonly ILogger<UpdateHitBatchHandler> _logger;
        private readonly IEventSourcingHandler<HitCollectionAggregate> _hitCollectionEventSourcingHandler;

        public UpdateHitBatchHandler(
            IMapper mapper,
            ILogger<UpdateHitBatchHandler> logger,
            IEventSourcingHandler<HitCollectionAggregate> hitCollectionEventSourcingHandler)
        {
            _mapper = mapper;
            _logger = logger;
            _hitCollectionEventSourcingHandler = hitCollectionEventSourcingHandler;
        }

        public async Task<List<HitVM>> Handle(UpdateHitBatchCommand request, CancellationToken cancellationToken)
        {
            var result = new List<HitVM>();

            // Process commands in chunks to prevent memory overflow and improve performance
            var commandBatches = request.Commands.Batch(BatchSize);

            foreach (var batch in commandBatches)
            {
                ValidateCommands(batch, request.RequestorUserId);

                var groupedCommands = batch.GroupBy(cmd => cmd.Id);

                foreach (var group in groupedCommands)
                {
                    var hitCollectionId = group.Key;

                    try
                    {
                        var aggregate = await _hitCollectionEventSourcingHandler.GetByAsyncId(hitCollectionId);

                        foreach (var command in group)
                        {
                            // Map command to domain event and apply update to the aggregate
                            var hitUpdatedEvent = _mapper.Map<HitUpdatedEvent>(command);
                            aggregate.UpdateHit(hitUpdatedEvent);

                            // Map the event to the entity and view model
                            var hit = _mapper.Map<Hit>(hitUpdatedEvent);
                            hit.HitCollectionId = hitCollectionId;
                            hit.Id = (Guid)command.HitId;

                            var hitViewModel = _mapper.Map<HitVM>(hit, opts => opts.Items["WithMeta"] = false);
                            result.Add(hitViewModel);
                        }

                        await _hitCollectionEventSourcingHandler.SaveAsync(aggregate);
                        _logger.LogInformation("✅ Updated {Count} hits for HitCollection {Id}", group.Count(), hitCollectionId);
                    }
                    catch (AggregateNotFoundException ex)
                    {
                        _logger.LogWarning(ex, "⚠️ Aggregate not found for HitCollectionId: {Id}", hitCollectionId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Unexpected error while updating HitCollectionId: {Id}", hitCollectionId);
                        // Optionally rethrow or propagate depending on retry strategy
                    }
                }
            }

            return result;
        }

        /*
         * Validates a batch of commands for consistency and authorization.
         * Throws ArgumentException for invalid data.
         */
        private void ValidateCommands(IEnumerable<UpdateHitCommand> commands, Guid requestorUserId)
        {
            foreach (var command in commands)
            {
                command.RequestorUserId = requestorUserId;
                command.SetUpdateProperties(requestorUserId);

                if (command.VoteToAdd != null)
                {
                    var (userId, vote) = command.VoteToAdd;

                    if (userId != requestorUserId.ToString())
                        throw new ArgumentException("User cannot cast a vote for another user");

                    if (!IsValidVote(vote))
                        throw new ArgumentException("Vote value must be Positive, Negative, or Neutral");
                }
            }
        }

        /*
         * Validates that the vote value is within accepted range.
         */
        private bool IsValidVote(string vote) =>
            vote == "Positive" || vote == "Negative" || vote == "Neutral";
    }
}
