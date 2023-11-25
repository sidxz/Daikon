using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Domain.Historical;
using Daikon.VersionStore.Settings;
using MongoDB.Driver;

namespace Daikon.VersionStore.Repositories
{
    public class VersionStoreRepository<VersionEntityModel> : IVersionStoreRepository<VersionEntityModel> where VersionEntityModel : BaseVersionEntity
    {
        private readonly IMongoCollection<VersionEntityModel> _eventStoreCollection;

        public VersionStoreRepository(IVersionDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _eventStoreCollection = database.GetCollection<VersionEntityModel>(settings.CollectionName);
        }

        public async Task<VersionEntityModel> GetByAsyncId(Guid entityId)
        {
            return await _eventStoreCollection.Find(x => x.Id == entityId)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<VersionEntityModel> GetByAsyncEntityId(Guid entityId)
        {
            return await _eventStoreCollection.Find(x => x.EntityId == entityId)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        // save async
        public async Task SaveAsync(VersionEntityModel newModel)
        {
            await _eventStoreCollection.InsertOneAsync(newModel).ConfigureAwait(false);
        }

        // update async
        public async Task UpdateAsync(VersionEntityModel newModel)
        {
            var replaceOptions = new ReplaceOptions { IsUpsert = true };
            await _eventStoreCollection.ReplaceOneAsync(x => x.EntityId == newModel.EntityId, newModel, replaceOptions).ConfigureAwait(false);
        }

    }
}