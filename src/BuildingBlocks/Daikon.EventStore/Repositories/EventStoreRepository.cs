using CQRS.Core.Domain;
using CQRS.Core.Event;
using Daikon.EventStore.Settings;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Daikon.EventStore.Repositories
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly IMongoCollection<EventModel> _eventStoreCollection;
        private readonly ILogger<EventStoreRepository> _logger;

        public EventStoreRepository(IEventDatabaseSettings settings, ILogger<EventStoreRepository> logger)
        {
            // Initialize MongoDB client and collection
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _eventStoreCollection = database.GetCollection<EventModel>(settings.CollectionName);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /*
         FindByAggregateId(Guid aggregateId):
         
         Retrieves events by aggregate ID.
        */
        public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
        {
            try
            {
                return await _eventStoreCollection
                    .Find(x => x.AggregateIdentifier == aggregateId)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching events by aggregate ID: {AggregateId}", aggregateId);
                throw new ApplicationException("An error occurred while fetching events by aggregate ID.", ex);
            }
        }

        /*
         SaveAsync(EventModel @event):
         
         Saves a single event asynchronously.
        */
        public async Task SaveAsync(EventModel @event)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            try
            {
                await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving event: {EventId}", @event.Id);
                throw new ApplicationException("An error occurred while saving the event.", ex);
            }
        }

        /*
         SaveBatchAsync(IEnumerable<EventModel> events):
         
         Saves multiple events asynchronously.
        */
        public async Task SaveBatchAsync(IEnumerable<EventModel> events)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));

            var eventList = events.ToList();
            if (!eventList.Any()) return; // No events to save

            try
            {
                await _eventStoreCollection.InsertManyAsync(eventList).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving batch of events.");
                throw new ApplicationException("An error occurred while saving events.", ex);
            }
        }

        /*
         GetHistoryAsync:

         Retrieves event history based on optional filters such as aggregateId, aggregateType, eventType, 
         with optional date range and result limit.
        */
        public async Task<List<EventModel>> GetHistoryAsync(
            Guid? aggregateId,
            string aggregateType,
            string eventType,
            DateTime? startDate,
            DateTime? endDate,
            int limit = 100)
        {
            if (limit <= 0)
                throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be greater than zero.");
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                throw new ArgumentException("Start date must be earlier than or equal to the end date.");

            var filterBuilder = Builders<EventModel>.Filter;
            var filters = new List<FilterDefinition<EventModel>>();

            // Apply filters as necessary
            if (startDate.HasValue)
                filters.Add(filterBuilder.Gte(x => x.TimeStamp, startDate.Value));

            if (endDate.HasValue)
                filters.Add(filterBuilder.Lte(x => x.TimeStamp, endDate.Value));

            if (aggregateId.HasValue)
                filters.Add(filterBuilder.Eq(x => x.AggregateIdentifier, aggregateId.Value));

            if (!string.IsNullOrWhiteSpace(aggregateType))
                filters.Add(filterBuilder.Eq(x => x.AggregateType, aggregateType));

            if (!string.IsNullOrWhiteSpace(eventType))
                filters.Add(filterBuilder.Eq(x => x.EventType, eventType));

            var combinedFilter = filters.Any() 
                ? filterBuilder.And(filters) 
                : FilterDefinition<EventModel>.Empty;

            try
            {
                return await _eventStoreCollection
                    .Find(combinedFilter)
                    .Limit(limit)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching event history.");
                throw new ApplicationException("An error occurred while fetching event history.", ex);
            }
        }
    }
}
