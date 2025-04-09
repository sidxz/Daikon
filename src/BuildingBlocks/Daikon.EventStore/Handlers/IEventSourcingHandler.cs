using Daikon.EventStore.Aggregate;

namespace Daikon.EventStore.Handlers
{
    public interface IEventSourcingHandler<TAggregate> where TAggregate : AggregateRoot
    {
        Task SaveAsync(AggregateRoot Root);
        Task<TAggregate> GetByAsyncId(Guid id);
    }
}