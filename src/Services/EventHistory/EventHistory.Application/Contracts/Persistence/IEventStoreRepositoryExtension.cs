

using Daikon.EventStore.Models;

namespace EventHistory.Application.Contracts.Persistence
{
    public interface IEventStoreRepositoryExtension
    {
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
            List<Guid> aggregateIds,
            List<string> aggregateTypes,
            List<string> eventTypes,
            DateTime? startDate,
            DateTime? endDate,
            int limit = 100
        );
    }
}