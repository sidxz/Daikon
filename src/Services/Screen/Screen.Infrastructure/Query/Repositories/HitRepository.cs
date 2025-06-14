
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Entities;

namespace Screen.Infrastructure.Query.Repositories
{
    public class HitRepository : IHitRepository
    {
        private readonly IMongoCollection<Hit> _hit;
        private readonly ILogger<HitRepository> _logger;

        public HitRepository(IConfiguration configuration, ILogger<HitRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("ScreenMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("ScreenMongoDbSettings:DatabaseName"));
            _hit = database.GetCollection<Hit>(configuration.GetValue<string>("ScreenMongoDbSettings:HitCollectionName"));
            _hit.Indexes.CreateOne(new CreateIndexModel<Hit>(Builders<Hit>.IndexKeys.Ascending(t => t.MoleculeId), new CreateIndexOptions { Unique = false }));
            _hit.Indexes.CreateOne(new CreateIndexModel<Hit>(Builders<Hit>.IndexKeys.Ascending(t => t.HitCollectionId), new CreateIndexOptions { Unique = false }));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        public async Task CreateHit(Hit hit)
        {

            ArgumentNullException.ThrowIfNull(hit);

            try
            {
                _logger.LogInformation("CreateHit: Creating hit {HitId}, {Hit}", hit.Id, hit.ToJson());
                await _hit.InsertOneAsync(hit);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the hit with ID {HitId}", hit.Id);
                throw new RepositoryException(nameof(HitRepository), "Error creating hit", ex);
            }
        }


        public async Task<Hit> ReadHitById(Guid id)
        {
            return await _hit.Find(hit => hit.Id == id).FirstOrDefaultAsync();
        }



        public async Task<List<Hit>> GetHitsList()
        {
            try
            {
                return await _hit.Find(hit => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the hit list");
                throw new RepositoryException(nameof(HitRepository), "Error getting hit list", ex);
            }

        }

        public async Task<List<Hit>> GetHitsListByHitCollectionId(Guid hitCollectionId)
        {
            try
            {
                return await _hit.Find(hit => hit.HitCollectionId == hitCollectionId).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the hit list");
                throw new RepositoryException(nameof(HitRepository), "Error getting hit list", ex);
            }

        }

        public async Task UpdateHit(Hit hit)
        {
            ArgumentNullException.ThrowIfNull(hit);

            try
            {
                _logger.LogInformation("UpdateHit: Updating hit {HitId}, {Hit}", hit.Id, hit.ToJson());
                await _hit.ReplaceOneAsync(t => t.Id == hit.Id, hit);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the hit with ID {HitId}", hit.Id);
                throw new RepositoryException(nameof(HitRepository), "Error updating hit", ex);
            }
        }

        public async Task DeleteHit(Guid hitId)
        {
            ArgumentNullException.ThrowIfNull(hitId);

            try
            {
                _logger.LogInformation("DeleteHit: Deleting hit {HitId}", hitId);
                await _hit.DeleteOneAsync(t => t.Id == hitId);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the hit with ID {HitId}", hitId);
                throw new RepositoryException(nameof(HitRepository), "Error deleting hit", ex);
            }
        }

        public async Task DeleteHitsByHitCollectionId(Guid hitCollectionId)
        {
            try
            {
                _logger.LogInformation("DeleteHitsByHitCollectionId: Deleting hits with hit collection ID {HitCollectionId}", hitCollectionId);

                await _hit.DeleteManyAsync(t => t.HitCollectionId == hitCollectionId);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the hits with hit collection ID {HitCollectionId}", hitCollectionId);
                throw new RepositoryException(nameof(HitRepository), "Error deleting hits", ex);
            }
        }

        public async Task DeleteHits(List<Guid> hitIds)
        {
            try
            {
                _logger.LogInformation("DeleteHits: Deleting hits with IDs {HitIds}", hitIds);
                await _hit.DeleteManyAsync(t => hitIds.Contains(t.Id));

            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the hits with IDs {HitIds}", hitIds);
                throw new RepositoryException(nameof(HitRepository), "Error deleting hits", ex);
            }
        }


        public async Task<List<Hit>> GetHitsWithRequestedMoleculeNameButNoMoleculeId()
        {
            try
            {
                _logger.LogInformation("Fetching hits where MoleculeId is null and RequestedMoleculeName is not null and exists.");

                var filter = Builders<Hit>.Filter.Eq(h => h.MoleculeId, null) &
                             Builders<Hit>.Filter.Ne(h => h.RequestedMoleculeName, null) &
                             Builders<Hit>.Filter.Exists(h => h.RequestedMoleculeName, true);

                return await _hit.Find(filter).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while querying hits with MoleculeId null and valid RequestedMoleculeName");
                throw new RepositoryException(nameof(HitRepository), "Error querying filtered hits", ex);
            }
        }

    }
}