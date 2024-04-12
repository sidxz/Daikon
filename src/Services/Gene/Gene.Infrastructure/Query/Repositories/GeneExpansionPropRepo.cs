
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Entities;
using Gene.Domain.EntityRevisions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Gene.Infrastructure.Query.Repositories
{
    public class GeneExpansionPropRepo : IGeneExpansionPropRepo
    {
        private readonly IMongoCollection<GeneExpansionProp> _expansionPropCollection;
        private readonly IVersionHub<GeneExpansionPropRevision> _versionHub;
        private readonly ILogger<GeneExpansionPropRepo> _logger;

        public GeneExpansionPropRepo(IConfiguration configuration, IVersionHub<GeneExpansionPropRevision> versionMaintainer, ILogger<GeneExpansionPropRepo> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName"));
            _expansionPropCollection = database.GetCollection<GeneExpansionProp>(
                configuration.GetValue<string>("GeneMongoDbSettings:GeneExpansionPropCollectionName") ??
                configuration.GetValue<string>("GeneMongoDbSettings:GeneCollectionName") + "GeneExpansionProp");

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(GeneExpansionProp geneExpansionProps)
        {
            ArgumentNullException.ThrowIfNull(geneExpansionProps);
            _logger.LogInformation("Create: Creating GeneExpansionProp {GeneExpansionPropId}, {geneExpansionProps}", geneExpansionProps.Id, geneExpansionProps.ToJson());

            try
            {
                await _expansionPropCollection.InsertOneAsync(geneExpansionProps);
                await _versionHub.CommitVersion(geneExpansionProps);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the gene with ID {GeneId}", geneExpansionProps.Id);
                throw new RepositoryException(nameof(GeneExpansionPropRepo), "Error creating gene", ex);
            }
        }

        public async Task Delete(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            _logger.LogInformation("Delete: Deleting GeneExpansionProp {GeneExpansionPropId}", id);
            try
            {
                await _expansionPropCollection.DeleteOneAsync(geneExpansionProps => geneExpansionProps.Id == id);
                await _versionHub.ArchiveEntity(id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the gene with ID {GeneId}", id);
                throw new RepositoryException(nameof(GeneExpansionPropRepo), "Error deleting gene", ex);
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
                _logger.LogError(ex, "An error occurred while deleting the gene with EntityId {EntityId}", entityId);
                throw new RepositoryException(nameof(GeneExpansionPropRepo), "Error deleting gene", ex);
            }
        }

        public async Task<List<GeneExpansionProp>> ListByEntityId(Guid entityId)
        {
            ArgumentNullException.ThrowIfNull(entityId);
            _logger.LogInformation("ListByEntityId: Listing GeneExpansionProps by EntityId {EntityId}", entityId);
            try
            {
                var res = await _expansionPropCollection.Find(geneExpansionProps => geneExpansionProps.GeneId == entityId).ToListAsync();
                return res;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while listing the gene with EntityId {EntityId}", entityId);
                throw new RepositoryException(nameof(GeneExpansionPropRepo), "Error listing gene", ex);
            }
        }

        public async Task<GeneExpansionProp> ReadById(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            _logger.LogInformation("ReadById: Reading GeneExpansionProp {GeneExpansionPropId}", id);
            try
            {
                var res = await _expansionPropCollection.Find(geneExpansionProps => geneExpansionProps.Id == id).FirstOrDefaultAsync();
                return res;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while reading the gene with ID {GeneId}", id);
                throw new RepositoryException(nameof(GeneExpansionPropRepo), "Error reading gene", ex);
            }
        }

        public async Task Update(GeneExpansionProp geneExpansionProps)
        {
            ArgumentNullException.ThrowIfNull(geneExpansionProps);
            _logger.LogInformation("Update: Updating GeneExpansionProp {GeneExpansionPropId}, {geneExpansionProps}", geneExpansionProps.Id, geneExpansionProps.ToJson());
            try
            {
                await _expansionPropCollection.ReplaceOneAsync(geneExpansionProps => geneExpansionProps.Id == geneExpansionProps.Id, geneExpansionProps);
                await _versionHub.CommitVersion(geneExpansionProps);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the gene with ID {GeneId}", geneExpansionProps.Id);
                throw new RepositoryException(nameof(GeneExpansionPropRepo), "Error updating gene", ex);
            }
        }
    }
}