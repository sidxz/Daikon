using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CQRS.Core.Event;

namespace CQRS.Core.Domain
{
    /// <summary>
    /// Defines the contract for interacting with the event store repository.
    /// Provides methods for saving, querying, and retrieving event models from the event store.
    /// </summary>
    public interface IEventStoreRepository
    {
        /// <summary>
        /// Saves a single event asynchronously into the event store.
        /// </summary>
        /// <param name="event">The event to be saved.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task SaveAsync(EventModel @event);

        /// <summary>
        /// Retrieves all events associated with a specific aggregate identifier asynchronously.
        /// </summary>
        /// <param name="aggregateId">The identifier of the aggregate for which to retrieve events.</param>
        /// <returns>A Task representing the asynchronous operation, with a list of matching events.</returns>
        Task<List<EventModel>> FindByAggregateId(Guid aggregateId);

        /// <summary>
        /// Saves a batch of events asynchronously into the event store.
        /// </summary>
        /// <param name="events">The collection of events to be saved.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task SaveBatchAsync(IEnumerable<EventModel> events);

        
    }
}
