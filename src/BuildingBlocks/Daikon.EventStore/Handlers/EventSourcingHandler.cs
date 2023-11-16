
using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;

/*
== Overview ==
The EventSourcingHandler<TAggregate> class, part of the Daikon.EventStore.Handlers namespace, is a generic handler 
for event sourcing operations in a CQRS (Command Query Responsibility Segregation) architecture. It is designed 
to work with aggregate roots of type TAggregate, which must inherit from AggregateRoot and have a parameterless constructor.

== Responsibilities ==
Event Sourcing Operations: This class is primarily responsible for retrieving and saving the state of aggregates through event sourcing.
Interface Implementation: It implements the IEventSourcingHandler<TAggregate> interface,
                          ensuring compliance with the defined contract for event sourcing handlers.

== Developer Notes ==
new() is a constraint that specifies that TAggregate must have a public parameterless constructor. 
This allows the EventSourcingHandler to create new instances of TAggregate using the new TAggregate() syntax.
*/

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