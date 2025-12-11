using Daikon.EventStore.Repositories;
using Daikon.Events.Targets;
using Target.Domain.Aggregates;
using Target.Domain.Exceptions;
using Target.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Target.Infrastructure.DomainServices
{
    public class TargetUniquenessChecker(IEventStoreRepository eventStoreRepository, ILogger<TargetUniquenessChecker> logger) : ITargetUniquenessChecker
    {
        private readonly IEventStoreRepository _eventStoreRepository = eventStoreRepository ?? throw new ArgumentNullException(nameof(eventStoreRepository));
        private readonly ILogger<TargetUniquenessChecker> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task EnsureTargetNameIsUniqueAsync(Guid strainId, string name, Guid? existingTargetId = null)
        {
            var aggregateIds = await _eventStoreRepository.GetAllAggregateIds().ConfigureAwait(false);

            foreach (var aggregateId in aggregateIds)
            {
                if (existingTargetId.HasValue && existingTargetId.Value == aggregateId)
                {
                    continue;
                }

                var events = await _eventStoreRepository.FindByAggregateId(aggregateId).ConfigureAwait(false);
                var createdEvent = events
                    .Where(@event => string.Equals(@event.AggregateType, nameof(TargetAggregate), StringComparison.OrdinalIgnoreCase))
                    .Select(@event => @event.EventData)
                    .OfType<TargetCreatedEvent>()
                    .FirstOrDefault();

                if (createdEvent == null)
                {
                    continue;
                }

                if (createdEvent.StrainId == strainId && string.Equals(createdEvent.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Duplicate target detected for strain {StrainId} with name {TargetName}", strainId, name);
                    throw new DomainInvariantViolationException($"Target with name '{name}' already exists for strain '{strainId}'.");
                }
            }
        }
    }
}
