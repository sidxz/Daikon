using CQRS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Infrastructure.Repositories
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly IMongoCollection<AppUser> _appUserCollection;
        private readonly ILogger<AppUserRepository> _logger;
        public AppUserRepository(IConfiguration configuration, ILogger<AppUserRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("UserStoreMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("UserStoreMongoDbSettings:DatabaseName"));
            _appUserCollection = database.GetCollection<AppUser>(configuration.GetValue<string>("UserStoreMongoDbSettings:AppUserCollectionName"));
            _appUserCollection.Indexes.CreateOne(new CreateIndexModel<AppUser>(Builders<AppUser>.IndexKeys.Ascending(t => t.NormalizedEmail), new CreateIndexOptions { Unique = true }));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task AddUser(AppUser user)
        {
            ArgumentNullException.ThrowIfNull(user);
            try
            {
                _logger.LogInformation("AddUser: Creating user {UserId}, {User}", user.Id, user.ToJson());
                await _appUserCollection.InsertOneAsync(user);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the user with ID {UserId}", user.Id);
                throw new RepositoryException(nameof(AppUserRepository), "Error creating user", ex);
            }
        }

        public async Task DeleteUser(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                _logger.LogInformation("DeleteUser: Deleting user {UserId}", id);
                await _appUserCollection.DeleteOneAsync(user => user.Id == id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the user with ID {UserId}", id);
                throw new RepositoryException(nameof(AppUserRepository), "Error deleting user", ex);
            }
        }

        public async Task<AppUser> GetUserByEmail(string email)
        {
            ArgumentNullException.ThrowIfNull(email);
            try
            {
                return await _appUserCollection.Find(user => user.NormalizedEmail == email.ToUpper()).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the user with email {Email}", email);
                throw new RepositoryException(nameof(AppUserRepository), "Error getting user", ex);
            }
        }

        public async Task<AppUser> GetUserById(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                return await _appUserCollection.Find(user => user.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the user with ID {UserId}", id);
                throw new RepositoryException(nameof(AppUserRepository), "Error getting user", ex);
            }
        }

        public async Task<AppUser> GetUserByOIDCSub(string OIDCSub)
        {
            ArgumentNullException.ThrowIfNull(OIDCSub);
            try
            {
                return await _appUserCollection.Find(user => user.OIDCSub == OIDCSub).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the user with OIDCSub {OIDCSub}", OIDCSub);
                throw new RepositoryException(nameof(AppUserRepository), "Error getting user", ex);
            }
        }

        public async Task<AppUser> GetUserByEntraObjectId(string EntraObjectId)
        {
            ArgumentNullException.ThrowIfNull(EntraObjectId);
            try
            {
                return await _appUserCollection.Find(user => user.EntraObjectId == EntraObjectId).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the user with EntraObjectId {EntraObjectId}", EntraObjectId);
                throw new RepositoryException(nameof(AppUserRepository), "Error getting user", ex);
            }
        }

        public async Task<List<AppUser>> GetUsersList()
        {
            try
            {
                return await _appUserCollection.Find(user => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the user list");
                throw new RepositoryException(nameof(AppUserRepository), "Error getting user list", ex);
            }
        }

        public async Task UpdateUser(AppUser user)
        {
            ArgumentNullException.ThrowIfNull(user);
            try
            {
                _logger.LogInformation("UpdateUser: Updating user {UserId}, {User}", user.Id, user.ToJson());
                await _appUserCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user with ID {UserId}", user.Id);
                throw new RepositoryException(nameof(AppUserRepository), "Error updating user", ex);
            }
        }

        public Task<List<AppUser>> GetUsersByRole(Guid roleId)
        {
            ArgumentNullException.ThrowIfNull(roleId);
            try {
                return _appUserCollection.Find(user => user.AppRoleIds.Contains(roleId)).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the users with role ID {RoleId}", roleId);
                throw new RepositoryException(nameof(AppUserRepository), "Error getting users by role", ex);
            }
        }
    }
}