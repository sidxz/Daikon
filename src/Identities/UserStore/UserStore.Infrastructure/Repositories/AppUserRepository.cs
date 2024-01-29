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
            _appUserCollection.Indexes.CreateOne(new CreateIndexModel<AppUser>(Builders<AppUser>.IndexKeys.Ascending(t => t.OIDCSub), new CreateIndexOptions { Unique = true }));
            _appUserCollection.Indexes.CreateOne(new CreateIndexModel<AppUser>(Builders<AppUser>.IndexKeys.Ascending(t => t.EntraObjectId), new CreateIndexOptions { Unique = true }));
            _appUserCollection.Indexes.CreateOne(new CreateIndexModel<AppUser>(Builders<AppUser>.IndexKeys.Ascending(t => t.NormalizedEmail), new CreateIndexOptions { Unique = true }));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public Task AddUser(AppUser user)
        {
            ArgumentNullException.ThrowIfNull(user);
            try
            {
                _logger.LogInformation("AddUser: Creating user {UserId}, {User}", user.Id, user.ToJson());
                return _appUserCollection.InsertOneAsync(user);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the user with ID {UserId}", user.Id);
                throw new RepositoryException(nameof(AppUserRepository), "Error creating user", ex);
            }
        }

        public Task DeleteUser(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                _logger.LogInformation("DeleteUser: Deleting user {UserId}", id);
                return _appUserCollection.DeleteOneAsync(user => user.Id == id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the user with ID {UserId}", id);
                throw new RepositoryException(nameof(AppUserRepository), "Error deleting user", ex);
            }
        }

        public Task<AppUser> GetUserByEmail(string email)
        {
            ArgumentNullException.ThrowIfNull(email);
            try
            {
                return _appUserCollection.Find(user => user.NormalizedEmail == email.ToUpper()).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the user with email {Email}", email);
                throw new RepositoryException(nameof(AppUserRepository), "Error getting user", ex);
            }
        }

        public Task<AppUser> GetUserById(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                return _appUserCollection.Find(user => user.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the user with ID {UserId}", id);
                throw new RepositoryException(nameof(AppUserRepository), "Error getting user", ex);
            }
        }

        public Task<AppUser> GetUserByOIDCSub(string OIDCSub)
        {
            ArgumentNullException.ThrowIfNull(OIDCSub);
            try
            {
                return _appUserCollection.Find(user => user.OIDCSub == OIDCSub).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the user with OIDCSub {OIDCSub}", OIDCSub);
                throw new RepositoryException(nameof(AppUserRepository), "Error getting user", ex);
            }
        }

        public Task<List<AppUser>> GetUsersList()
        {
            try
            {
                return _appUserCollection.Find(user => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the user list");
                throw new RepositoryException(nameof(AppUserRepository), "Error getting user list", ex);
            }
        }

        public Task UpdateUser(AppUser user)
        {
            ArgumentNullException.ThrowIfNull(user);
            try
            {
                _logger.LogInformation("UpdateUser: Updating user {UserId}, {User}", user.Id, user.ToJson());
                return _appUserCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user with ID {UserId}", user.Id);
                throw new RepositoryException(nameof(AppUserRepository), "Error updating user", ex);
            }
        }
    }
}