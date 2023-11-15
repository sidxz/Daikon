
using CQRS.Core.Domain;
using CQRS.Core.Event;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Gene.Infrastructure.Command.Repositories
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly IMongoCollection<EventModel> _eventStoreCollection;

        public EventStoreRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("EventDatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("EventDatabaseSettings:DatabaseName"));
            _eventStoreCollection = database.GetCollection<EventModel>(configuration.GetValue<string>("EventDatabaseSettings:CollectionName"));
        }

        public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
        {
            return await _eventStoreCollection.Find(x => x.AggregateIdentifier == aggregateId)
                            .ToListAsync().ConfigureAwait(false);
        }

        public async Task SaveAsync(EventModel @event)
        {
            await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
        }

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