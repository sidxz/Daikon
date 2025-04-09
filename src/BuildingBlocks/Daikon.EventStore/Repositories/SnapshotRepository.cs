using Daikon.EventStore.Models;
using Daikon.EventStore.Settings;
using MongoDB.Driver;

namespace Daikon.EventStore.Repositories
{
    public class SnapshotRepository : ISnapshotRepository
    {
        private readonly IMongoCollection<SnapshotModel> _snapshotCollection;

        public SnapshotRepository(IEventDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DatabaseName);
            _snapshotCollection = db.GetCollection<SnapshotModel>("AggregateSnapshots");

            _snapshotCollection.Indexes.CreateOne(
                new CreateIndexModel<SnapshotModel>(
                    Builders<SnapshotModel>.IndexKeys
                        .Ascending(x => x.AggregateIdentifier)
                        .Descending(x => x.Version)
                )
            );
        }

        public async Task<SnapshotModel?> GetLatestSnapshotAsync(Guid aggregateId)
        {
            return await _snapshotCollection
                .Find(x => x.AggregateIdentifier == aggregateId)
                .SortByDescending(x => x.Version)
                .FirstOrDefaultAsync();
        }

        public async Task SaveSnapshotAsync(SnapshotModel snapshot)
        {
            await _snapshotCollection.InsertOneAsync(snapshot);
        }
    }
}
