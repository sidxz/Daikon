
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
    public class GeneProteinProductionRepository : IGeneProteinProductionRepository
    {

        private readonly IMongoCollection<ProteinProduction> _proteinProductionCollection; 
        private readonly IVersionHub<ProteinProductionRevision> _versionHub;
        private readonly ILogger<GeneProteinProductionRepository> _logger;

        public GeneProteinProductionRepository(IConfiguration configuration, IVersionHub<ProteinProductionRevision> versionMaintainer, ILogger<GeneProteinProductionRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName"));
            _proteinProductionCollection = database.GetCollection<ProteinProduction>(
                configuration.GetValue<string>("GeneMongoDbSettings:GeneProteinProductionCollectionName") ?? 
                configuration.GetValue<string>("GeneMongoDbSettings:GeneCollectionName") + "ProteinProduction");

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddProteinProduction(ProteinProduction proteinProduction)
        {

            ArgumentNullException.ThrowIfNull(proteinProduction);

            try
            {
                _logger.LogInformation("AddProteinProduction: Creating ProteinProduction {ProteinProductionId}, {proteinProduction}", proteinProduction.Id, proteinProduction.ToJson());
                await _proteinProductionCollection.InsertOneAsync(proteinProduction);
                await _versionHub.CommitVersion(proteinProduction);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the gene with ID {GeneId}", proteinProduction.Id);
                throw new RepositoryException(nameof(GeneProteinProductionRepository), "Error creating gene", ex);
            }
        }



        public async Task<ProteinProduction> Read(Guid id)
        {
            return await _proteinProductionCollection.Find(proteinProduction => proteinProduction.Id == id).FirstOrDefaultAsync();
        }




        public async Task<List<ProteinProduction>> GetProteinProductionList()
        {
            try
            {
                return await _proteinProductionCollection.Find(essential => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the essential list");
                throw new RepositoryException(nameof(GeneProteinProductionRepository), "Error getting essential list", ex);
            }

        }

        public async Task<List<ProteinProduction>> GetProteinProductionOfGene(Guid geneId)
        {
            try
            {
                return await _proteinProductionCollection.Find(proteinProduction => proteinProduction.GeneId == geneId).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the proteinProduction list");
                throw new RepositoryException(nameof(GeneProteinProductionRepository), "Error getting proteinProduction list", ex);
            }

        }

        public async Task UpdateProteinProduction(ProteinProduction proteinProduction)
        {
            ArgumentNullException.ThrowIfNull(proteinProduction);

            var filter = Builders<ProteinProduction>.Filter.Eq(e => e.Id, proteinProduction.Id);
            var update = Builders<ProteinProduction>.Update
                .Set(e => e.GeneId, proteinProduction.GeneId)
                .Set(e => e.Production, proteinProduction.Production)
                .Set(e => e.Method, proteinProduction.Method)
                .Set(e => e.Purity, proteinProduction.Purity)
                .Set(e => e.DateProduced, proteinProduction.DateProduced)
                .Set(e => e.PMID, proteinProduction.PMID)
                .Set(e => e.Notes, proteinProduction.Notes)
                .Set(e => e.URL, proteinProduction.URL);
            try
            {
                _logger.LogInformation("UpdateProteinProduction: Updating proteinProduction {proteinProductionId}, {proteinProduction}", proteinProduction.Id, proteinProduction.ToJson());
                await _proteinProductionCollection.UpdateOneAsync(filter, update);
                await _versionHub.CommitVersion(proteinProduction);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the proteinProduction with ID {proteinProductionId}", proteinProduction.Id);
                throw new RepositoryException(nameof(GeneProteinProductionRepository), "Error updating gene", ex);
            }

        }


        public async Task DeleteProteinProduction(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                _logger.LogInformation("DeleteProteinProduction: Deleting ProteinProduction {ProteinProduction}", id);
                await _proteinProductionCollection.DeleteOneAsync(gene => gene.Id == id);
                await _versionHub.ArchiveEntity(id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the ProteinProduction with ID {ProteinProduction}", id);
                throw new RepositoryException(nameof(GeneProteinProductionRepository), "Error deleting ProteinProduction", ex);
            }

        }

        public async Task DeleteAllProteinProductionsOfGene(Guid geneId)
        {
            ArgumentNullException.ThrowIfNull(geneId);

            // find all proteinProductions of gene and archive them individually
            var proteinProductions = await _proteinProductionCollection.Find(proteinProduction => proteinProduction.GeneId == geneId).ToListAsync();
            foreach (var proteinProduction in proteinProductions)
            {
                _logger.LogInformation("DeleteProteinProductionsOfGene: Archiving ProteinProduction {ProteinProductionId}", proteinProduction.Id);
                await _versionHub.ArchiveEntity(proteinProduction.Id);
            }
            // delete all proteinProductions of gene
            try
            {
                _logger.LogInformation("DeleteProteinProductionsOfGene: Deleting ProteinProductions of Gene {GeneId}", geneId);
                await _proteinProductionCollection.DeleteManyAsync(proteinProduction => proteinProduction.GeneId == geneId);
                
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the ProteinProductions of Gene with ID {GeneId}", geneId);
                throw new RepositoryException(nameof(GeneProteinProductionRepository), "Error deleting ProteinProductions of Gene", ex);
            }

        }



        public async Task<ProteinProductionRevision> GetProteinProductionRevisions(Guid Id)
        {
            var proteinProductionRevision = await _versionHub.GetVersions(Id);
            return proteinProductionRevision;
        }

        
    }
}