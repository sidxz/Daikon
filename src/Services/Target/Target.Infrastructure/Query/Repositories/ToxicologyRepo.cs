
using Daikon.EventStore.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Target.Application.Contracts.Persistence;
using Target.Domain.Entities;

namespace Target.Infrastructure.Query.Repositories
{
    public class ToxicologyRepo : IToxicologyRepo
    {
        private readonly IMongoCollection<Toxicology> _toxicologyCollection;
        private readonly ILogger<ToxicologyRepo> _logger;

        public ToxicologyRepo(IConfiguration configuration, ILogger<ToxicologyRepo> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("TargetMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("TargetMongoDbSettings:DatabaseName"));

            _toxicologyCollection = database.GetCollection<Toxicology>
                (configuration.GetValue<string>("TargetMongoDbSettings:TargetToxicologyCollectionName") ?? "TargetToxicologies");
            _toxicologyCollection.Indexes.CreateOne
                (new CreateIndexModel<Toxicology>(Builders<Toxicology>.IndexKeys.Ascending(t => t.TargetId), new CreateIndexOptions { Unique = false }));
                
            _toxicologyCollection.Indexes.CreateOne
                (new CreateIndexModel<Toxicology>(Builders<Toxicology>.IndexKeys.Ascending(t => t.Topic), new CreateIndexOptions { Unique = false }));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }
        public async Task<Toxicology> Create(Toxicology toxicology)
        {
            ArgumentNullException.ThrowIfNull(toxicology);
            try
            {
                await _toxicologyCollection.InsertOneAsync(toxicology);
                return toxicology;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error in CreateToxicology");
                throw;
            }
        }

        public async Task Delete(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                await _toxicologyCollection.DeleteOneAsync(t => t.ToxicologyId == id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error in DeleteToxicology");
                throw;
            }
        }

        public async Task DeleteByTargetId(Guid targetId)
        {
            ArgumentNullException.ThrowIfNull(targetId);
            try
            {
                await _toxicologyCollection.DeleteManyAsync(t => t.TargetId == targetId);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error in DeleteToxicologyByTargetId");
                throw;
            }
        }

        public async Task<List<Toxicology>> ReadAll()
        {
            try
            {
                return await _toxicologyCollection.Find(t => true)
                .SortBy(t => t.TargetId)
                .ThenBy(t => t.Topic)
                .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error in ReadAllToxicologies");
                throw;
            }
        }

        public async Task<Toxicology> ReadById(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                return await _toxicologyCollection.Find(t => t.ToxicologyId == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error in ReadToxicologyById");
                throw;
            }
        }

        public async Task<List<Toxicology>> ReadByTargetId(Guid targetId)
        {
            ArgumentNullException.ThrowIfNull(targetId);
            try
            {
                return await _toxicologyCollection.Find(t => t.TargetId == targetId)
                .SortBy(t => t.Topic)
                .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error in ReadToxicologyByTargetId");
                throw;
            }
        }

        public async Task<Toxicology> Update(Toxicology toxicology)
        {
            _logger.LogInformation($"Updating Toxicology: {toxicology.ToxicologyId}");
            _logger.LogInformation($"Toxicology: {toxicology.ToJson()}");

            ArgumentNullException.ThrowIfNull(toxicology);
            try
            {
                await _toxicologyCollection.ReplaceOneAsync(t => t.ToxicologyId == toxicology.ToxicologyId, toxicology);
                return toxicology;

            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error in UpdateToxicology");
                throw;
            }
        }
    }
}