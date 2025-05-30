using Daikon.EventStore.Models;
using Daikon.EventStore.Settings;
using MongoDB.Driver;

namespace Daikon.EventStore.Repositories
{
    /*
     SnapshotRepository handles storing and retrieving aggregate snapshots in MongoDB.
     Used to optimize aggregate rehydration by avoiding full event stream replay.
    */
    public class SnapshotRepository : ISnapshotRepository
    {
        private readonly IMongoCollection<SnapshotModel> _snapshotCollection;

        /*
         Constructor initializes the MongoDB collection and ensures indexes are created.
        */
        public SnapshotRepository(IEventDatabaseSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DatabaseName);

            _snapshotCollection = db.GetCollection<SnapshotModel>(RepositoryConstants.SnapshotCollectionName);

            /* Create compound index to optimize latest snapshot lookup */
            _snapshotCollection.Indexes.CreateOne(
                new CreateIndexModel<SnapshotModel>(
                    Builders<SnapshotModel>.IndexKeys
                        .Ascending(x => x.AggregateIdentifier)
                        .Descending(x => x.Version)
                )
            );
        }

        /*
         Retrieves the most recent snapshot for a given aggregate ID.
         Returns null if no snapshot is found.
        */
        public async Task<SnapshotModel?> GetLatestSnapshotAsync(Guid aggregateId)
        {
            return await _snapshotCollection
                .Find(x => x.AggregateIdentifier == aggregateId)
                .SortByDescending(x => x.Version)
                .FirstOrDefaultAsync();
        }

        /*
         Persists a snapshot to the snapshot collection.
        */
        public async Task SaveSnapshotAsync(SnapshotModel snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            await _snapshotCollection.InsertOneAsync(snapshot);
        }
    }
}
