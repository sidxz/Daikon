using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Event;
using Daikon.EventStore.Settings;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

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