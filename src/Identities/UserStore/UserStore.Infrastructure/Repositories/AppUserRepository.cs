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
        private static int _emailIndexEnsured; // the index is created once per process, not once per request-scoped instance
        public AppUserRepository(IConfiguration configuration, ILogger<AppUserRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("UserStoreMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("UserStoreMongoDbSettings:DatabaseName"));
            _appUserCollection = database.GetCollection<AppUser>(configuration.GetValue<string>("UserStoreMongoDbSettings:AppUserCollectionName"));
            if (Interlocked.CompareExchange(ref _emailIndexEnsured, 1, 0) == 0)
            {
                try
                {
                    _appUserCollection.Indexes.CreateOne(new CreateIndexModel<AppUser>(Builders<AppUser>.IndexKeys.Ascending(t => t.NormalizedEmail), new CreateIndexOptions { Unique = true }));
                }
                catch
                {
                    Interlocked.Exchange(ref _emailIndexEnsured, 0); // retried by the next request
                    throw;
                }
            }
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

        public async Task<bool> LinkIdentity(Guid userId, string? entraObjectId, string? oidcSub)
        {
            var f = Builders<AppUser>.Filter;
            var filter = f.Eq(u => u.Id, userId) & f.Eq(u => u.IsUserLocked, false);
            var update = Builders<AppUser>.Update
                .Set(u => u.IsOIDCConnected, true)
                .Set(u => u.OIDCConnectionDate, DateTime.UtcNow);
            if (entraObjectId is not null)
            {
                filter &= f.Eq(u => u.EntraObjectId, null!) | f.Eq(u => u.EntraObjectId, entraObjectId);
                update = update.Set(u => u.EntraObjectId, entraObjectId);
            }
            if (oidcSub is not null)
            {
                filter &= f.Eq(u => u.OIDCSub, null!) | f.Eq(u => u.OIDCSub, oidcSub);
                update = update.Set(u => u.OIDCSub, oidcSub);
            }
            try
            {
                // One conditional write: nothing else on the record is rewritten, and a concurrent lock or link wins
                var result = await _appUserCollection.UpdateOneAsync(filter, update);
                return result.MatchedCount == 1;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while linking the identity of user {UserId}", userId);
                throw new RepositoryException(nameof(AppUserRepository), "Error linking identity", ex);
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