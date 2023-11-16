
using CQRS.Core.Domain;
using CQRS.Core.Event;
using Daikon.EventStore.Settings;
using MongoDB.Driver;

/* 
== Overview ==
The EventStoreRepository class, part of the Daikon.EventStore.Repositories namespace, is responsible for 
interacting with a MongoDB database to store and retrieve event data. 
It implements the IEventStoreRepository interface, providing asynchronous methods for saving and querying EventModel objects.

== Key Components ==
IMongoCollection<EventModel> _eventStoreCollection: A MongoDB collection that holds EventModel objects.
*/
namespace Daikon.EventStore.Repositories
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly IMongoCollection<EventModel> _eventStoreCollection;

        public EventStoreRepository(IEventDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _eventStoreCollection = database.GetCollection<EventModel>(settings.CollectionName);
        }

        /*
         FindByAggregateId(Guid aggregateId):

            Asynchronously retrieves a list of EventModel objects from the MongoDB collection based on the specified aggregate identifier.
            Returns a Task<List<EventModel>> representing the asynchronous operation.
        */
        public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
        {
            return await _eventStoreCollection.Find(x => x.AggregateIdentifier == aggregateId)
                            .ToListAsync().ConfigureAwait(false);
        }

        /*
         InsertOneAsync(EventModel @event):

            Asynchronously inserts a single EventModel object into the MongoDB collection.
            Returns a Task representing the asynchronous operation.
        */
        public async Task SaveAsync(EventModel @event)
        {
            await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
        }

        /*
         InsertManyAsync(IEnumerable<EventModel> events):

            Asynchronously inserts a batch of EventModel objects into the MongoDB collection.
            Returns a Task representing the asynchronous operation.
        */
        public async Task SaveBatchAsync(IEnumerable<EventModel> events)
        {
            if (events == null || !events.Any())
            {
                return; // No events to save
            }

            await _eventStoreCollection
                .InsertManyAsync(events)
                .ConfigureAwait(false);
        }

    }
}