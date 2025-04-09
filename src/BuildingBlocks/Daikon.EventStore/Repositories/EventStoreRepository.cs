using Daikon.EventStore.Models;
using Daikon.EventStore.Settings;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Daikon.EventStore.Repositories
{
    /*
     EventStoreRepository is responsible for interacting with the MongoDB event store.
     It supports storing and retrieving event streams for a given aggregate.
    */
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly IMongoCollection<EventModel> _eventStoreCollection;
        private readonly ILogger<EventStoreRepository> _logger;

        /*
         Constructor sets up the MongoDB collection and indexes.
        */
        public EventStoreRepository(IEventDatabaseSettings settings, ILogger<EventStoreRepository> logger)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            /* Initialize MongoDB client and select database and collection */
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _eventStoreCollection = database.GetCollection<EventModel>(RepositoryConstants.EventStoreCollectionName);

            /* Define and create necessary indexes for performance */
            var indexModels = new List<CreateIndexModel<EventModel>>
            {
                new CreateIndexModel<EventModel>(
                    Builders<EventModel>.IndexKeys.Descending(e => e.TimeStamp),
                    new CreateIndexOptions { Unique = false }
                ),
                new CreateIndexModel<EventModel>(
                    Builders<EventModel>.IndexKeys.Ascending(e => e.AggregateIdentifier),
                    new CreateIndexOptions { Unique = false }
                ),
                new CreateIndexModel<EventModel>(
                    Builders<EventModel>.IndexKeys.Ascending(e => e.AggregateType),
                    new CreateIndexOptions { Unique = false }
                ),
                new CreateIndexModel<EventModel>(
                    Builders<EventModel>.IndexKeys.Ascending(e => e.EventType),
                    new CreateIndexOptions { Unique = false }
                ),
                new CreateIndexModel<EventModel>(
                    Builders<EventModel>.IndexKeys
                        .Ascending(e => e.AggregateIdentifier)
                        .Ascending(e => e.AggregateType)
                        .Ascending(e => e.EventType)
                        .Descending(e => e.TimeStamp),
                    new CreateIndexOptions { Unique = false }
                )
            };

            _eventStoreCollection.Indexes.CreateMany(indexModels);
        }

        /*
         Finds all events for a given aggregate ID and returns them ordered by version.
        */
        public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
        {
            try
            {
                return await _eventStoreCollection
                    .Find(x => x.AggregateIdentifier == aggregateId)
                    .SortBy(x => x.Version)
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
         Saves a single event to the event store asynchronously.
        */
        public async Task SaveAsync(EventModel @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            try
            {
                await _eventStoreCollection
                    .InsertOneAsync(@event)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving event: {EventId}", @event.Id);
                throw new ApplicationException("An error occurred while saving the event.", ex);
            }
        }

        /*
         Saves a batch of events to the event store asynchronously.
         Skips operation if the list is empty.
        */
        public async Task SaveBatchAsync(IEnumerable<EventModel> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            var eventList = events.ToList();
            if (!eventList.Any())
                return;

            try
            {
                await _eventStoreCollection
                    .InsertManyAsync(eventList)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving batch of events.");
                throw new ApplicationException("An error occurred while saving events.", ex);
            }
        }
    }
}
