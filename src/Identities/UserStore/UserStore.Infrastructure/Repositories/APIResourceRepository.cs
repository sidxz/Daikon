
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Infrastructure.Repositories
{
    public class APIResourceRepository : IAPIResourceRepository
    {
        private readonly IMongoCollection<APIResource> _appUserCollection;
        private readonly ILogger<APIResourceRepository> _logger;
        public APIResourceRepository(IConfiguration configuration, ILogger<APIResourceRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("UserStoreMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("UserStoreMongoDbSettings:DatabaseName"));
            _appUserCollection = database.GetCollection<APIResource>(configuration.GetValue<string>("UserStoreMongoDbSettings:APIResourceCollectionName"));
            _appUserCollection.Indexes.CreateOne(new CreateIndexModel<APIResource>(Builders<APIResource>.IndexKeys.Ascending(r => r.Endpoint), new CreateIndexOptions { Unique = false }));
            _appUserCollection.Indexes.CreateOne(new CreateIndexModel<APIResource>(Builders<APIResource>.IndexKeys.Ascending(r => r.Method), new CreateIndexOptions { Unique = false }));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task AddAPIResource(APIResource resource)
        {
            ArgumentNullException.ThrowIfNull(resource);
            try
            {
                _logger.LogInformation("AddAPIResource: Creating resource {ResourceId}, {Resource}", resource.Id, resource.ToJson());
                await _appUserCollection.InsertOneAsync(resource);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the resource with ID {ResourceId}", resource.Id);
                throw new RepositoryException(nameof(APIResourceRepository), "Error creating resource", ex);
            }
        }

        public async Task DeleteAPIResource(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                _logger.LogInformation("DeleteAPIResource: Deleting resource {ResourceId}", id);
                await _appUserCollection.DeleteOneAsync(resource => resource.Id == id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the resource with ID {ResourceId}", id);
                throw new RepositoryException(nameof(APIResourceRepository), "Error deleting resource", ex);
            }
        }

        public async Task<APIResource> GetAPIResourceByEndPoint(string method, string endpoint)
        {
            ArgumentNullException.ThrowIfNull(endpoint);
            try
            {
                _logger.LogInformation("GetAPIResourceByEndPoint: Getting resource {Method} : {Endpoint}", method, endpoint);
                return await _appUserCollection.Find(resource => resource.Method == method && resource.Endpoint == endpoint).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the resource with endpoint {Endpoint}", endpoint);
                throw new RepositoryException(nameof(APIResourceRepository), "Error getting resource", ex);
            }
        }

        public async Task<APIResource> GetAPIResourceById(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                _logger.LogInformation("GetAPIResourceById: Getting resource {ResourceId}", id);
                return await _appUserCollection.Find(resource => resource.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the resource with ID {ResourceId}", id);
                throw new RepositoryException(nameof(APIResourceRepository), "Error getting resource", ex);
            }
        }

        public async Task<List<APIResource>> GetAPIResourcesByService(string service)
        {
            ArgumentNullException.ThrowIfNull(service);
            try
            {
                _logger.LogInformation("GetAPIResourceByService: Getting resource {Service}", service);
                return await _appUserCollection.Find(resource => resource.Service == service).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the resource with service {Service}", service);
                throw new RepositoryException(nameof(APIResourceRepository), "Error getting resource", ex);
            }
        }

        public async Task<List<APIResource>> GetAPIResourcesList()
        {
            try
            {
                _logger.LogInformation("GetAPIResourcesList: Getting all resources");
                return await _appUserCollection.Find(resource => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting all resources");
                throw new RepositoryException(nameof(APIResourceRepository), "Error getting resources", ex);
            }
        }

        public async Task UpdateAPIResource(APIResource resource)
        {
            ArgumentNullException.ThrowIfNull(resource);
            try
            {
                _logger.LogInformation("UpdateAPIResource: Updating resource {ResourceId}, {Resource}", resource.Id, resource.ToJson());
                await _appUserCollection.ReplaceOneAsync(r => r.Id == resource.Id, resource);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the resource with ID {ResourceId}", resource.Id);
                throw new RepositoryException(nameof(APIResourceRepository), "Error updating resource", ex);
            }
        }
    }
}