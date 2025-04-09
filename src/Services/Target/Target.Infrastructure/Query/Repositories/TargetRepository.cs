
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Target.Application.Contracts.Persistence;

namespace Target.Infrastructure.Query.Repositories
{
    public class TargetRepository : ITargetRepository
    {
        private readonly IMongoCollection<Domain.Entities.Target> _targetCollection;
        private readonly ILogger<TargetRepository> _logger;

        public TargetRepository(IConfiguration configuration, ILogger<TargetRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("TargetMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("TargetMongoDbSettings:DatabaseName"));
            _targetCollection = database.GetCollection<Domain.Entities.Target>(configuration.GetValue<string>("TargetMongoDbSettings:TargetCollectionName"));
            _targetCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Target>(Builders<Domain.Entities.Target>.IndexKeys.Ascending(t => t.Name), new CreateIndexOptions { Unique = false }));
            _targetCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Target>(Builders<Domain.Entities.Target>.IndexKeys.Ascending(t => t.StrainId), new CreateIndexOptions { Unique = false }));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        public async Task CreateTarget(Domain.Entities.Target target)
        {

            ArgumentNullException.ThrowIfNull(target);

            try
            {
                _logger.LogInformation("CreateTarget: Creating target {TargetId}, {Target}", target.Id, target.ToJson());
                await _targetCollection.InsertOneAsync(target);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the target with ID {TargetId}", target.Id);
                throw new RepositoryException(nameof(TargetRepository), "Error creating target", ex);
            }
        }


        public async Task<Domain.Entities.Target> ReadTargetById(Guid id)
        {
            return await _targetCollection.Find(target => target.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Domain.Entities.Target> ReadTargetByName(string name)
        {
            return await _targetCollection.Find(target => target.Name == name).FirstOrDefaultAsync();
        }


        public async Task<List<Domain.Entities.Target>> GetTargetsList()
        {
            try
            {
                return await _targetCollection.Find(target => true)
                .SortBy(target => target.Name)
                .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the target list");
                throw new RepositoryException(nameof(TargetRepository), "Error getting target list", ex);
            }

        }

        public async Task<List<Domain.Entities.Target>> GetTargetsListByStrainId(Guid strainId)
        {
            try
            {
                return await _targetCollection.Find(target => target.StrainId == strainId).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the target list");
                throw new RepositoryException(nameof(TargetRepository), "Error getting target list", ex);
            }

        }



        public async Task UpdateTarget(Domain.Entities.Target target)
        {
            ArgumentNullException.ThrowIfNull(target);

            try
            {
                _logger.LogInformation("UpdateTarget: Updating target {TargetId}, {Target}", target.Id, target.ToJson());
                await _targetCollection.ReplaceOneAsync(t => t.Id == target.Id, target);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the target with ID {TargetId}", target.Id);
                throw new RepositoryException(nameof(TargetRepository), "Error updating target", ex);
            }
        }

        public async Task DeleteTarget(Domain.Entities.Target target)
        {
            ArgumentNullException.ThrowIfNull(target);

            try
            {
                _logger.LogInformation("DeleteTarget: Deleting target {TargetId}, {Target}", target.Id, target.ToJson());
                await _targetCollection.DeleteOneAsync(t => t.Id == target.Id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the target with ID {TargetId}", target.Id);
                throw new RepositoryException(nameof(TargetRepository), "Error deleting target", ex);
            }
        }
    }
}