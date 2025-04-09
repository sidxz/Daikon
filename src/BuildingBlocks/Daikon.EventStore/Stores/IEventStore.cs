
using Daikon.EventStore.Aggregate;
using Daikon.EventStore.Event;

namespace Daikon.EventStore.Stores
{
    public interface IEventStore<TAggregate> where TAggregate : AggregateRoot
    {
        Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion);
        Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId);
        Task<List<BaseEvent>> GetEventsAfterVersionAsync(Guid aggregateId, int version);
    }
}


