
using CQRS.Core.Exceptions;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Gene.Infrastructure.Query.Repositories
{
    public class GeneExpansionPropRepo : IGeneExpansionPropRepo
    {
        private readonly IMongoCollection<GeneExpansionProp> _expansionPropCollection;
        private readonly ILogger<GeneExpansionPropRepo> _logger;

        public GeneExpansionPropRepo(IConfiguration configuration, ILogger<GeneExpansionPropRepo> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName"));
            _expansionPropCollection = database.GetCollection<GeneExpansionProp>(
                configuration.GetValue<string>("GeneMongoDbSettings:GeneExpansionPropCollectionName") ??
                configuration.GetValue<string>("GeneMongoDbSettings:GeneCollectionName") + "GeneExpansionProp");
            
            _expansionPropCollection.Indexes.CreateOne
                (new CreateIndexModel<GeneExpansionProp>(Builders<GeneExpansionProp>.IndexKeys.Ascending(t => t.DateCreated), new CreateIndexOptions { Unique = false }));
                
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(GeneExpansionProp geneExpansionProps)
        {
            ArgumentNullException.ThrowIfNull(geneExpansionProps);
            _logger.LogInformation("Create: Creating GeneExpansionProp {GeneExpansionPropId}, {geneExpansionProps}", geneExpansionProps.Id, geneExpansionProps.ToJson());

            try
            {
                await _expansionPropCollection.InsertOneAsync(geneExpansionProps);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the GeneExpansionProps with ID {id}", geneExpansionProps.Id);
                throw new RepositoryException(nameof(GeneExpansionPropRepo), "Error creating GeneExpansionProps", ex);
            }
        }

        public async Task Delete(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            _logger.LogInformation("Delete: Deleting GeneExpansionProp {GeneExpansionPropId}", id);
            try
            {
                await _expansionPropCollection.DeleteOneAsync(geneExpansionProps => geneExpansionProps.Id == id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the GeneExpansionProps with ID {id}", id);
                throw new RepositoryException(nameof(GeneExpansionPropRepo), "Error deleting GeneExpansionProps", ex);
            }
        }

        public Task DeleteAllOfEntity(Guid entityId)
        {
            ArgumentNullException.ThrowIfNull(entityId);
            _logger.LogInformation("DeleteAllOfEntity: Deleting all GeneExpansionProps of EntityId {EntityId}", entityId);
            try {
                return _expansionPropCollection.DeleteManyAsync(geneExpansionProps => geneExpansionProps.GeneId == entityId);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the GeneExpansionProps of entity with EntityId {EntityId}", entityId);
                throw new RepositoryException(nameof(GeneExpansionPropRepo), "Error deleting GeneExpansionProps", ex);
            }
        }

        public async Task<List<GeneExpansionProp>> ListByEntityId(Guid entityId)
        {
            ArgumentNullException.ThrowIfNull(entityId);
            try
            {
                var res = await _expansionPropCollection.Find(geneExpansionProps => geneExpansionProps.GeneId == entityId)
                .SortBy(geneExpansionProps => geneExpansionProps.DateCreated)
                .ToListAsync();
                return res;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while listing the GeneExpansionProps with EntityId {EntityId}", entityId);
                throw new RepositoryException(nameof(GeneExpansionPropRepo), "Error listing GeneExpansionProps", ex);
            }
        }

        public async Task<GeneExpansionProp> ReadById(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                var res = await _expansionPropCollection.Find(geneExpansionProps => geneExpansionProps.Id == id).FirstOrDefaultAsync();
                return res;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while reading the GeneExpansionProps with ID {id}", id);
                throw new RepositoryException(nameof(GeneExpansionPropRepo), "Error reading GeneExpansionProps", ex);
            }
        }

        public async Task<GeneExpansionProp> FindByTypeAndValue(Guid Id, string type, string value)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(value);
            try
            {
                var res = await _expansionPropCollection.Find
                    (geneExpansionProps => 
                        geneExpansionProps.GeneId == Id &&
                        geneExpansionProps.ExpansionType == type && 
                        geneExpansionProps.ExpansionValue.Value == value).FirstOrDefaultAsync();
                return res;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while finding the GeneExpansionProps with Type {type} and Value {value}", type, value);
                throw new RepositoryException(nameof(GeneExpansionPropRepo), "Error finding GeneExpansionProps", ex);
            }
        }

        public async Task Update(GeneExpansionProp geneExpansionProps)
        {
            ArgumentNullException.ThrowIfNull(geneExpansionProps);
            _logger.LogInformation("Update: Updating GeneExpansionProp {GeneExpansionPropId}, {geneExpansionProps}", geneExpansionProps.Id, geneExpansionProps.ToJson());
            try
            {
                await _expansionPropCollection.ReplaceOneAsync(p => p.Id == geneExpansionProps.Id, geneExpansionProps);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the GeneExpansionProps with ID {id}", geneExpansionProps.Id);
                throw new RepositoryException(nameof(GeneExpansionPropRepo), "Error updating GeneExpansionProps", ex);
            }
        }
    }
}