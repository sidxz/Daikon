
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Gene.Infrastructure.Query.Repositories
{
    public class GeneProteinActivityAssayRepository : IGeneProteinActivityAssayRepository
    {

        private readonly IMongoCollection<ProteinActivityAssay> _proteinActivityAssayCollection;
        private readonly ILogger<GeneProteinActivityAssayRepository> _logger;

        public GeneProteinActivityAssayRepository(IConfiguration configuration, ILogger<GeneProteinActivityAssayRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName"));
            _proteinActivityAssayCollection = database.GetCollection<ProteinActivityAssay>(
                configuration.GetValue<string>("GeneMongoDbSettings:GeneProteinActivityAssayCollectionName") ??
                configuration.GetValue<string>("GeneMongoDbSettings:GeneCollectionName") + "ProteinActivityAssay");

            _proteinActivityAssayCollection.Indexes.CreateOne
                (new CreateIndexModel<ProteinActivityAssay>(Builders<ProteinActivityAssay>.IndexKeys.Ascending(t => t.DateCreated), new CreateIndexOptions { Unique = false }));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<ProteinActivityAssay> Read(Guid id)
        {
            try
            {
                return await _proteinActivityAssayCollection.Find(proteinActivityAssay => proteinActivityAssay.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the proteinActivityAssay with ID {ProteinActivityAssayId}", id);
                throw new RepositoryException(nameof(GeneProteinActivityAssayRepository), "Error getting proteinActivityAssay", ex);
            }
        }


        public async Task<List<ProteinActivityAssay>> GetProteinActivityAssayList()
        {
            try
            {
                return await _proteinActivityAssayCollection.Find(proteinActivityAssay => true)
                .SortBy(proteinActivityAssay => proteinActivityAssay.DateCreated)
                .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the proteinActivityAssay list");
                throw new RepositoryException(nameof(GeneProteinActivityAssayRepository), "Error getting proteinActivityAssay list", ex);
            }

        }

        public async Task<List<ProteinActivityAssay>> GetProteinActivityAssayOfGene(Guid geneId)
        {
            try
            {
                return await _proteinActivityAssayCollection.Find(proteinActivityAssay => proteinActivityAssay.GeneId == geneId)
                .SortBy(proteinActivityAssay => proteinActivityAssay.DateCreated)
                .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the proteinActivityAssay list");
                throw new RepositoryException(nameof(GeneProteinActivityAssayRepository), "Error getting proteinActivityAssay list", ex);
            }
        }

        public async Task AddProteinActivityAssay(ProteinActivityAssay proteinActivityAssay)
        {

            ArgumentNullException.ThrowIfNull(proteinActivityAssay);

            try
            {
                _logger.LogInformation("AddProteinActivityAssay: Creating ProteinActivityAssay {ProteinActivityAssayId}, {proteinActivityAssay}", proteinActivityAssay.Id, proteinActivityAssay.ToJson());
                await _proteinActivityAssayCollection.InsertOneAsync(proteinActivityAssay);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the proteinActivityAssay with ID {id}", proteinActivityAssay.Id);
                throw new RepositoryException(nameof(GeneProteinActivityAssayRepository), "Error creating proteinActivityAssay", ex);
            }
        }

        public async Task UpdateProteinActivityAssay(ProteinActivityAssay proteinActivityAssay)
        {
            ArgumentNullException.ThrowIfNull(proteinActivityAssay);

            try
            {
                _logger.LogInformation("UpdateProteinActivityAssay: Updating ProteinActivityAssay {ProteinActivityAssayId}, {proteinActivityAssay}", proteinActivityAssay.Id, proteinActivityAssay.ToJson());
                await _proteinActivityAssayCollection.ReplaceOneAsync(p => p.Id == proteinActivityAssay.Id, proteinActivityAssay);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the ProteinActivityAssay with ID {ProteinActivityAssay}", proteinActivityAssay.Id);
                throw new RepositoryException(nameof(GeneProteinActivityAssayRepository), "Error updating ProteinActivityAssay", ex);
            }
        }


        public async Task DeleteProteinActivityAssay(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                _logger.LogInformation("DeleteProteinActivityAssay: Deleting ProteinActivityAssay {ProteinActivityAssay}", id);
                await _proteinActivityAssayCollection.DeleteOneAsync(p => p.Id == id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the ProteinActivityAssay with ID {ProteinActivityAssay}", id);
                throw new RepositoryException(nameof(GeneProteinActivityAssayRepository), "Error deleting ProteinActivityAssay", ex);
            }

        }

        public async Task DeleteAllProteinActivityAssaysOfGene(Guid geneId)
        {
            ArgumentNullException.ThrowIfNull(geneId);

            // find all proteinActivityAssays of gene and archive them individually
            var proteinActivityAssays = await _proteinActivityAssayCollection.Find(proteinActivityAssay => proteinActivityAssay.GeneId == geneId).ToListAsync();
            foreach (var proteinActivityAssay in proteinActivityAssays)
            {
                _logger.LogInformation("DeleteProteinActivityAssaysOfGene: Archiving ProteinActivityAssay {ProteinActivityAssayId}", proteinActivityAssay.Id);
            }
            // delete all proteinActivityAssays of gene
            try
            {
                _logger.LogInformation("DeleteProteinActivityAssaysOfGene: Deleting ProteinActivityAssays of Gene {GeneId}", geneId);
                await _proteinActivityAssayCollection.DeleteManyAsync(proteinActivityAssay => proteinActivityAssay.GeneId == geneId);

            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the ProteinActivityAssays of Gene with ID {GeneId}", geneId);
                throw new RepositoryException(nameof(GeneProteinActivityAssayRepository), "Error deleting ProteinActivityAssays of Gene", ex);
            }

        }
    }
}