using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;

namespace Daikon.EventStore.Handlers
{
    public class EventSourcingHandler<TAggregate> : IEventSourcingHandler<TAggregate> where TAggregate : AggregateRoot, new()
    {
        private readonly IEventStore<TAggregate> _eventStore;

        public EventSourcingHandler(IEventStore<TAggregate> eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<TAggregate> GetByAsyncId(Guid aggregateId)

        {
            var aggregate = new TAggregate();
            var events = await _eventStore.GetEventsAsync(aggregateId);

            if (events == null || !events.Any())
            {
                return aggregate;
            }

            aggregate.ReplayEvents(events);
            aggregate.Version = events.Select(x => x.Version).Max();


            return aggregate;
        }

        public async Task SaveAsync(AggregateRoot aggregate)
        {
            await _eventStore.SaveEventAsync(aggregate.Id, aggregate.GetUncommittedChanges(), aggregate.Version);
            aggregate.MarkChangesAsCommitted();
        }
    }
}