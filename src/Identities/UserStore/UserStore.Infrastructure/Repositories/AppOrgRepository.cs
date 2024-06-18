
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Infrastructure.Repositories
{
    public class AppOrgRepository : IAppOrgRepository
    {
        private readonly IMongoCollection<AppOrg> _appOrgCollection;
        private readonly ILogger<AppOrgRepository> _logger;

        public AppOrgRepository(IConfiguration configuration, ILogger<AppOrgRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("UserStoreMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("UserStoreMongoDbSettings:DatabaseName"));
            _appOrgCollection = database.GetCollection<AppOrg>(configuration.GetValue<string>("UserStoreMongoDbSettings:AppOrgCollectionName"));
            _appOrgCollection.Indexes.CreateOne(new CreateIndexModel<AppOrg>(Builders<AppOrg>.IndexKeys.Ascending(t => t.Name), new CreateIndexOptions { Unique = true }));
            _appOrgCollection.Indexes.CreateOne(new CreateIndexModel<AppOrg>(Builders<AppOrg>.IndexKeys.Ascending(t => t.Alias), new CreateIndexOptions { Unique = true }));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddOrg(AppOrg org)
        {
            ArgumentNullException.ThrowIfNull(org);
            try
            {
                _logger.LogInformation("AddOrg: Creating org {OrgId}, {Org}", org.Id, org.ToJson());
                await _appOrgCollection.InsertOneAsync(org);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the org with ID {OrgId}", org.Id);
                throw new RepositoryException(nameof(AppOrgRepository), "Error creating org", ex);
            }
        }

        public async Task DeleteOrg(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                _logger.LogInformation("DeleteOrg: Deleting org {OrgId}", id);
                await _appOrgCollection.DeleteOneAsync(org => org.Id == id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the org with ID {OrgId}", id);
                throw new RepositoryException(nameof(AppOrgRepository), "Error deleting org", ex);
            }
        }

        public async Task<AppOrg> GetOrgById(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                _logger.LogInformation("GetOrgById: Getting org by ID {OrgId}", id);
                return await _appOrgCollection.Find(org => org.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the org with ID {OrgId}", id);
                throw new RepositoryException(nameof(AppOrgRepository), "Error getting org", ex);
            }
        }

        public async Task<AppOrg> GetOrgByAlias(string alias)
        {
            ArgumentNullException.ThrowIfNull(alias);
            try
            {
                _logger.LogInformation("GetOrgByAlias: Getting org by alias {Alias}", alias);
                return await _appOrgCollection.Find(org => org.Alias == alias).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the org with alias {Alias}", alias);
                throw new RepositoryException(nameof(AppOrgRepository), "Error getting org", ex);
            }
        }

        public async Task<AppOrg> GetOrgByName(string name)
        {
            ArgumentNullException.ThrowIfNull(name);
            try
            {
                _logger.LogInformation("GetOrgByName: Getting org by name {Name}", name);
                return await _appOrgCollection.Find(org => org.Name == name).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the org with name {Name}", name);
                throw new RepositoryException(nameof(AppOrgRepository), "Error getting org", ex);
            }
        }

        public async Task<List<AppOrg>> GetOrgsList()
        {
            try
            {
                _logger.LogInformation("GetOrgsList: Getting orgs list");
                return await _appOrgCollection.Find(new BsonDocument()).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the orgs list");
                throw new RepositoryException(nameof(AppOrgRepository), "Error getting orgs list", ex);
            }
        }

        public async Task<List<AppOrg>> GetOrgsListExcludeInternal()
        {
            try
            {
                _logger.LogInformation("GetOrgsListExcludeInternal: Getting orgs list");
                return await _appOrgCollection.Find(org => org.IsInternal == false).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the orgs list");
                throw new RepositoryException(nameof(AppOrgRepository), "Error getting orgs list", ex);
            }
        }

        public async Task UpdateOrg(AppOrg org)
        {
            ArgumentNullException.ThrowIfNull(org);
            try
            {
                _logger.LogInformation("UpdateOrg: Updating org {OrgId}, {Org}", org.Id, org.ToJson());
                await _appOrgCollection.ReplaceOneAsync(o => o.Id == org.Id, org);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the org with ID {OrgId}", org.Id);
                throw new RepositoryException(nameof(AppOrgRepository), "Error updating org", ex);
            }
        }
    }
}