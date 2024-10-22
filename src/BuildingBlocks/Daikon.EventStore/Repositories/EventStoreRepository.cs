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

       

    }
}
