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

        /// <summary>
        /// Retrieves event history based on multiple optional filters such as aggregateId, aggregateType, eventType,
        /// date range, and a result limit.
        /// </summary>
        /// <param name="aggregateId">The optional aggregate identifier to filter events by.</param>
        /// <param name="aggregateType">The optional aggregate type to filter events by.</param>
        /// <param name="eventType">The optional event type to filter events by.</param>
        /// <param name="startDate">The optional start date for filtering by timestamp.</param>
        /// <param name="endDate">The optional end date for filtering by timestamp.</param>
        /// <param name="limit">The optional limit on the number of events returned, defaulting to 100.</param>
        /// <returns>A Task representing the asynchronous operation, with a list of matching events.</returns>
        Task<List<EventModel>> GetHistoryAsync(
            Guid? aggregateId,
            string aggregateType,
            string eventType,
            DateTime? startDate,
            DateTime? endDate,
            int limit = 100
        );
    }
}
