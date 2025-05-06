using System.Linq.Expressions;
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace UserPreferences.Infrastructure.Repositories
{
    public abstract class MongoRepositoryBase<T> where T : class
    {
        protected readonly IMongoCollection<T> _collection;
        protected readonly ILogger _logger;

        protected MongoRepositoryBase(IConfiguration config, string collectionName, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(config);
            if (string.IsNullOrWhiteSpace(collectionName)) throw new ArgumentNullException(nameof(collectionName));
            ArgumentNullException.ThrowIfNull(logger);

            var connectionString = config.GetValue<string>("UserPreferencesMongoDbSettings:ConnectionString");
            var dbName = config.GetValue<string>("UserPreferencesMongoDbSettings:DatabaseName");

            if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(dbName))
            {
                throw new ArgumentException("MongoDB connection string or database name is not configured properly.");
            }

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            _collection = database.GetCollection<T>(collectionName);
            _logger = logger;
        }

        public virtual async Task InsertAsync(T entity)
        {
            try
            {
                await _collection.InsertOneAsync(entity);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Insert failed for {Entity}", typeof(T).Name);
                throw new RepositoryException(nameof(MongoRepositoryBase<T>), $"Error inserting {typeof(T).Name}", ex);
            }
        }

        public virtual async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync();
        }

        public virtual async Task<List<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public virtual async Task ReplaceAsync(Expression<Func<T, bool>> predicate, T entity)
        {
            try
            {
                await _collection.ReplaceOneAsync(predicate, entity, new ReplaceOptions { IsUpsert = true });
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Replace failed for {Entity}", typeof(T).Name);
                throw new RepositoryException(nameof(MongoRepositoryBase<T>), $"Error replacing {typeof(T).Name}", ex);
            }
        }

        public virtual async Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                await _collection.DeleteOneAsync(predicate);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Delete failed for {Entity}", typeof(T).Name);
                throw new RepositoryException(nameof(MongoRepositoryBase<T>), $"Error deleting {typeof(T).Name}", ex);
            }
        }
    }
}
