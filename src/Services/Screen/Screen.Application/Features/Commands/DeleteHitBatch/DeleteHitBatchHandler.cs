using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Screens;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Domain.Aggregates;

namespace Screen.Application.Features.Commands.DeleteHitBatch
{
    public class DeleteHitBatchHandler : IRequestHandler<DeleteHitBatchCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteHitBatchHandler> _logger;
        private readonly IEventSourcingHandler<HitCollectionAggregate> _hitCollectionEventSourcingHandler;

        public DeleteHitBatchHandler(
            IMapper mapper,
            ILogger<DeleteHitBatchHandler> logger,
            IEventSourcingHandler<HitCollectionAggregate> hitCollectionEventSourcingHandler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hitCollectionEventSourcingHandler = hitCollectionEventSourcingHandler ?? throw new ArgumentNullException(nameof(hitCollectionEventSourcingHandler));
        }

        public async Task<Unit> Handle(DeleteHitBatchCommand request, CancellationToken cancellationToken)
        {
            var grouped = request.Commands.GroupBy(cmd => cmd.Id); // Group by HitCollectionId

            foreach (var group in grouped)
            {
                var hitCollectionId = group.Key;

                try
                {
                    var aggregate = await _hitCollectionEventSourcingHandler.GetByAsyncId(hitCollectionId);

                    foreach (var cmd in group)
                    {
                        cmd.RequestorUserId = request.RequestorUserId;
                        cmd.SetUpdateProperties(request.RequestorUserId);
                        var evt = _mapper.Map<HitDeletedEvent>(cmd);
                        aggregate.DeleteHit(evt);
                    }

                    await _hitCollectionEventSourcingHandler.SaveAsync(aggregate);
                    _logger.LogInformation("✅ Deleted {Count} hits from HitCollection {Id}", group.Count(), hitCollectionId);
                }
                catch (AggregateNotFoundException ex)
                {
                    _logger.LogWarning(ex, "⚠️ Aggregate not found for HitCollectionId: {Id}", hitCollectionId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Failed to delete hits for HitCollectionId: {Id}", hitCollectionId);
                }
            }

            return Unit.Value;
        }
    }
}
