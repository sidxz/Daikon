using CQRS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Infrastructure.Repositories
{
    public class AppRoleRepository : IAppRoleRepository
    {
        private readonly IMongoCollection<AppRole> _appRoleCollection;
        private readonly ILogger<AppRoleRepository> _logger;
        public AppRoleRepository(IConfiguration configuration, ILogger<AppRoleRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("UserStoreMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("UserStoreMongoDbSettings:DatabaseName"));
            _appRoleCollection = database.GetCollection<AppRole>(configuration.GetValue<string>("UserStoreMongoDbSettings:AppRoleCollectionName"));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task AddRole(AppRole role)
        {
            ArgumentNullException.ThrowIfNull(role, nameof(role));
            role.NormalizedName = role.Name.ToUpperInvariant();

            try
            {
                _logger.LogInformation("AddRole: Creating role {RoleId}, {Role}", role.Id, role.ToJson());
                await _appRoleCollection.InsertOneAsync(role);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the role with ID {RoleId}", role.Id);
                throw new RepositoryException(nameof(AppRoleRepository), "Error creating role", ex);
            }
        }

        public async Task DeleteRole(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));

            try
            {
                _logger.LogInformation("DeleteRole: Deleting role {RoleId}", id);
                await _appRoleCollection.DeleteOneAsync(role => role.Id == id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the role with ID {RoleId}", id);
                throw new RepositoryException(nameof(AppRoleRepository), "Error deleting role", ex);
            }
        }

        public async Task<AppRole> GetRoleById(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));

            try
            {
                _logger.LogInformation("GetRoleById: Getting role {RoleId}", id);
                return await _appRoleCollection.Find(role => role.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the role with ID {RoleId}", id);
                throw new RepositoryException(nameof(AppRoleRepository), "Error getting role", ex);
            }
        }

        public async Task<AppRole> GetRoleByName(string name)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));

            try
            {
                _logger.LogInformation("GetRoleByName: Getting role {RoleName}", name);
                return await _appRoleCollection.Find(role => role.NormalizedName == name.ToUpperInvariant()).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the role with name {RoleName}", name);
                throw new RepositoryException(nameof(AppRoleRepository), "Error getting role", ex);
            }
        }

        public Task<List<AppRole>> GetRolesByIds(List<Guid> ids)
        {
            ArgumentNullException.ThrowIfNull(ids, nameof(ids));

            try
            {
                _logger.LogInformation("GetRolesByIds: Getting roles {RoleIds}", ids);
                return _appRoleCollection.Find(role => ids.Contains(role.Id)).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the roles with IDs {RoleIds}", ids);
                throw new RepositoryException(nameof(AppRoleRepository), "Error getting roles", ex);
            }
        }

        public async Task<List<AppRole>> GetRolesList()
        {
            try
            {
                _logger.LogInformation("GetRolesList: Getting all roles");
                return await _appRoleCollection.Find(role => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting all roles");
                throw new RepositoryException(nameof(AppRoleRepository), "Error getting roles", ex);
            }
        }

        public async Task UpdateRole(AppRole role)
        {
            ArgumentNullException.ThrowIfNull(role, nameof(role));
            role.NormalizedName = role.Name.ToUpperInvariant();

            try
            {
                _logger.LogInformation("UpdateRole: Updating role {RoleId}, {Role}", role.Id, role.ToJson());
                await _appRoleCollection.ReplaceOneAsync(r => r.Id == role.Id, role);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the role with ID {RoleId}", role.Id);
                throw new RepositoryException(nameof(AppRoleRepository), "Error updating role", ex);
            }
        }
    }
}