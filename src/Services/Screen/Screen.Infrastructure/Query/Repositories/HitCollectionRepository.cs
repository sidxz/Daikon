
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Entities;

namespace Screen.Infrastructure.Query.Repositories
{
    public class HitCollectionRepository : IHitCollectionRepository
    {
        private readonly IMongoCollection<HitCollection> _hitCollectionCollection;
        private readonly ILogger<HitCollectionRepository> _logger;

        public HitCollectionRepository(IConfiguration configuration, ILogger<HitCollectionRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("ScreenMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("ScreenMongoDbSettings:DatabaseName"));
            _hitCollectionCollection = database.GetCollection<HitCollection>(configuration.GetValue<string>("ScreenMongoDbSettings:HitCollectionCollectionName"));
            _hitCollectionCollection.Indexes.CreateOne(new CreateIndexModel<HitCollection>(Builders<HitCollection>.IndexKeys.Ascending(t => t.Name), new CreateIndexOptions { Unique = false }));
            _hitCollectionCollection.Indexes.CreateOne(new CreateIndexModel<HitCollection>(Builders<HitCollection>.IndexKeys.Ascending(t => t.ScreenId), new CreateIndexOptions { Unique = false }));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        public async Task CreateHitCollection(HitCollection hitCollection)
        {

            ArgumentNullException.ThrowIfNull(hitCollection);

            try
            {
                _logger.LogInformation("CreateHitCollection: Creating hitCollection {HitCollectionId}, {HitCollection}", hitCollection.Id, hitCollection.ToJson());
                await _hitCollectionCollection.InsertOneAsync(hitCollection);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the hitCollection with ID {HitCollectionId}", hitCollection.Id);
                throw new RepositoryException(nameof(HitCollectionRepository), "Error creating hitCollection", ex);
            }
        }


        public async Task<HitCollection> ReadHitCollectionById(Guid id)
        {
            return await _hitCollectionCollection.Find(hitCollection => hitCollection.Id == id).FirstOrDefaultAsync();
        }

        public async Task<HitCollection> ReadHitCollectionByName(string name)
        {
            return await _hitCollectionCollection.Find(hitCollection => hitCollection.Name == name).FirstOrDefaultAsync();
        }


        public async Task<List<HitCollection>> GetHitCollectionsList()
        {
            try
            {
                return await _hitCollectionCollection.Find(hitCollection => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the hitCollection list");
                throw new RepositoryException(nameof(HitCollectionRepository), "Error getting hitCollection list", ex);
            }

        }

        public async Task<List<HitCollection>> GetHitCollectionsListByScreenId(Guid screenId)
        {
            try
            {
                return await _hitCollectionCollection.Find(hitCollection => hitCollection.ScreenId == screenId).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the hitCollection list");
                throw new RepositoryException(nameof(HitCollectionRepository), "Error getting hitCollection list", ex);
            }

        }



        public async Task UpdateHitCollection(HitCollection hitCollection)
        {
            ArgumentNullException.ThrowIfNull(hitCollection);

            try
            {
                _logger.LogInformation("UpdateHitCollection: Updating hitCollection {HitCollectionId}, {HitCollection}", hitCollection.Id, hitCollection.ToJson());
                await _hitCollectionCollection.ReplaceOneAsync(t => t.Id == hitCollection.Id, hitCollection);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the hitCollection with ID {HitCollectionId}", hitCollection.Id);
                throw new RepositoryException(nameof(HitCollectionRepository), "Error updating hitCollection", ex);
            }
        }

        public async Task DeleteHitCollection(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                _logger.LogInformation("DeleteHitCollection: Deleting hitCollection {HitCollectionId}", id);
                await _hitCollectionCollection.DeleteOneAsync(t => t.Id == id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the hitCollection with ID {HitCollectionId}", id);
                throw new RepositoryException(nameof(HitCollectionRepository), "Error deleting hitCollection", ex);
            }
        }

    }
}