
using CQRS.Core.Domain;

namespace CQRS.Core.Handlers
{
    public interface IEventSourcingHandler<T>
    {
        Task SaveAsync(AggregateRoot Root);
        Task<T> GetByAsyncId(Guid id);
    }
}