
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
    public class GeneProteinActivityAssayRepository : IGeneProteinActivityAssayRepository
    {

        private readonly IMongoCollection<ProteinActivityAssay> _proteinActivityAssayCollection; 
        private readonly IVersionHub<ProteinActivityAssayRevision> _versionHub;
        private readonly ILogger<GeneProteinActivityAssayRepository> _logger;

        public GeneProteinActivityAssayRepository(IConfiguration configuration, IVersionHub<ProteinActivityAssayRevision> versionMaintainer, ILogger<GeneProteinActivityAssayRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName"));
            _proteinActivityAssayCollection = database.GetCollection<ProteinActivityAssay>(
                configuration.GetValue<string>("GeneMongoDbSettings:GeneProteinActivityAssayCollectionName") ?? 
                configuration.GetValue<string>("GeneMongoDbSettings:GeneCollectionName") + "ProteinActivityAssay");

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddProteinActivityAssay(ProteinActivityAssay proteinActivityAssay)
        {

            ArgumentNullException.ThrowIfNull(proteinActivityAssay);

            try
            {
                _logger.LogInformation("AddProteinActivityAssay: Creating ProteinActivityAssay {ProteinActivityAssayId}, {proteinActivityAssay}", proteinActivityAssay.Id, proteinActivityAssay.ToJson());
                await _proteinActivityAssayCollection.InsertOneAsync(proteinActivityAssay);
                await _versionHub.CommitVersion(proteinActivityAssay);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the gene with ID {GeneId}", proteinActivityAssay.Id);
                throw new RepositoryException(nameof(GeneProteinActivityAssayRepository), "Error creating gene", ex);
            }
        }



        public async Task<ProteinActivityAssay> Read(Guid id)
        {
            return await _proteinActivityAssayCollection.Find(proteinActivityAssay => proteinActivityAssay.Id == id).FirstOrDefaultAsync();
        }




        public async Task<List<ProteinActivityAssay>> GetProteinActivityAssayList()
        {
            try
            {
                return await _proteinActivityAssayCollection.Find(proteinActivityAssay => true).ToListAsync();
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
                return await _proteinActivityAssayCollection.Find(proteinActivityAssay => proteinActivityAssay.GeneId == geneId).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the proteinActivityAssay list");
                throw new RepositoryException(nameof(GeneProteinActivityAssayRepository), "Error getting proteinActivityAssay list", ex);
            }

        }

        public async Task UpdateProteinActivityAssay(ProteinActivityAssay proteinActivityAssay)
        {
            ArgumentNullException.ThrowIfNull(proteinActivityAssay);

            var filter = Builders<ProteinActivityAssay>.Filter.Eq(e => e.Id, proteinActivityAssay.Id);
            var update = Builders<ProteinActivityAssay>.Update
                .Set(e => e.Assay, proteinActivityAssay.Assay)
                .Set(e => e.Method, proteinActivityAssay.Method)
                .Set(e => e.Throughput, proteinActivityAssay.Throughput)
                .Set(e => e.PMID, proteinActivityAssay.PMID)
                .Set(e => e.Reference, proteinActivityAssay.Reference)
                .Set(e => e.URL, proteinActivityAssay.URL);
            try
            {
                _logger.LogInformation("UpdateProteinActivityAssay: Updating proteinActivityAssay {proteinActivityAssayId}, {proteinActivityAssay}", proteinActivityAssay.Id, proteinActivityAssay.ToJson());
                await _proteinActivityAssayCollection.UpdateOneAsync(filter, update);
                await _versionHub.CommitVersion(proteinActivityAssay);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the proteinActivityAssay with ID {proteinActivityAssayId}", proteinActivityAssay.Id);
                throw new RepositoryException(nameof(GeneProteinActivityAssayRepository), "Error updating gene", ex);
            }

        }


        public async Task DeleteProteinActivityAssay(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                _logger.LogInformation("DeleteProteinActivityAssay: Deleting ProteinActivityAssay {ProteinActivityAssay}", id);
                await _proteinActivityAssayCollection.DeleteOneAsync(gene => gene.Id == id);
                await _versionHub.ArchiveEntity(id);
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
                await _versionHub.ArchiveEntity(proteinActivityAssay.Id);
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



        public async Task<ProteinActivityAssayRevision> GetProteinActivityAssayRevisions(Guid Id)
        {
            var proteinActivityAssayRevision = await _versionHub.GetVersions(Id);
            return proteinActivityAssayRevision;
        }
    }
}