
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
    public class GeneCrispriStrainRepository : IGeneCrispriStrainRepository
    {

        private readonly IMongoCollection<CrispriStrain> _crispriStrainCollection; 
        private readonly IVersionHub<CrispriStrainRevision> _versionHub;
        private readonly ILogger<GeneCrispriStrainRepository> _logger;

        public GeneCrispriStrainRepository(IConfiguration configuration, IVersionHub<CrispriStrainRevision> versionMaintainer, ILogger<GeneCrispriStrainRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName"));
            _crispriStrainCollection = database.GetCollection<CrispriStrain>(
                configuration.GetValue<string>("GeneMongoDbSettings:GeneCrispriStrainCollectionName") ?? 
                configuration.GetValue<string>("GeneMongoDbSettings:GeneCollectionName") + "CrispriStrain");

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddCrispriStrain(CrispriStrain crispriStrain)
        {

            ArgumentNullException.ThrowIfNull(crispriStrain);

            try
            {
                _logger.LogInformation("AddCrispriStrain: Creating CrispriStrain {CrispriStrainId}, {crispriStrain}", crispriStrain.Id, crispriStrain.ToJson());
                await _crispriStrainCollection.InsertOneAsync(crispriStrain);
                await _versionHub.CommitVersion(crispriStrain);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the gene with ID {GeneId}", crispriStrain.Id);
                throw new RepositoryException(nameof(GeneCrispriStrainRepository), "Error creating gene", ex);
            }
        }



        public async Task<CrispriStrain> Read(Guid id)
        {
            return await _crispriStrainCollection.Find(crispriStrain => crispriStrain.Id == id).FirstOrDefaultAsync();
        }




        public async Task<List<CrispriStrain>> GetCrispriStrainList()
        {
            try
            {
                return await _crispriStrainCollection.Find(crispriStrain => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the crispriStrain list");
                throw new RepositoryException(nameof(GeneCrispriStrainRepository), "Error getting crispriStrain list", ex);
            }

        }

        public async Task<List<CrispriStrain>> GetCrispriStrainOfGene(Guid geneId)
        {
            try
            {
                return await _crispriStrainCollection.Find(crispriStrain => crispriStrain.GeneId == geneId).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the crispriStrain list");
                throw new RepositoryException(nameof(GeneCrispriStrainRepository), "Error getting crispriStrain list", ex);
            }

        }

        public async Task UpdateCrispriStrain(CrispriStrain crispriStrain)
        {
            ArgumentNullException.ThrowIfNull(crispriStrain);

            var filter = Builders<CrispriStrain>.Filter.Eq(e => e.Id, crispriStrain.Id);
            var update = Builders<CrispriStrain>.Update
                .Set(e => e.CrispriStrainName, crispriStrain.CrispriStrainName)
                .Set(e => e.Notes, crispriStrain.Notes);
            try
            {
                _logger.LogInformation("UpdateCrispriStrain: Updating crispriStrain {crispriStrainId}, {crispriStrain}", crispriStrain.Id, crispriStrain.ToJson());
                await _crispriStrainCollection.UpdateOneAsync(filter, update);
                await _versionHub.CommitVersion(crispriStrain);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the crispriStrain with ID {crispriStrainId}", crispriStrain.Id);
                throw new RepositoryException(nameof(GeneCrispriStrainRepository), "Error updating gene", ex);
            }

        }


        public async Task DeleteCrispriStrain(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                _logger.LogInformation("DeleteCrispriStrain: Deleting CrispriStrain {CrispriStrain}", id);
                await _crispriStrainCollection.DeleteOneAsync(gene => gene.Id == id);
                await _versionHub.ArchiveEntity(id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the CrispriStrain with ID {CrispriStrain}", id);
                throw new RepositoryException(nameof(GeneCrispriStrainRepository), "Error deleting CrispriStrain", ex);
            }

        }

        public async Task DeleteAllCrispriStrainsOfGene(Guid geneId)
        {
            ArgumentNullException.ThrowIfNull(geneId);

            // find all crispriStrains of gene and archive them individually
            var crispriStrains = await _crispriStrainCollection.Find(crispriStrain => crispriStrain.GeneId == geneId).ToListAsync();
            foreach (var crispriStrain in crispriStrains)
            {
                _logger.LogInformation("DeleteCrispriStrainsOfGene: Archiving CrispriStrain {CrispriStrainId}", crispriStrain.Id);
                await _versionHub.ArchiveEntity(crispriStrain.Id);
            }
            // delete all crispriStrains of gene
            try
            {
                _logger.LogInformation("DeleteCrispriStrainsOfGene: Deleting CrispriStrains of Gene {GeneId}", geneId);
                await _crispriStrainCollection.DeleteManyAsync(crispriStrain => crispriStrain.GeneId == geneId);
                
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the CrispriStrains of Gene with ID {GeneId}", geneId);
                throw new RepositoryException(nameof(GeneCrispriStrainRepository), "Error deleting CrispriStrains of Gene", ex);
            }

        }



        public async Task<CrispriStrainRevision> GetCrispriStrainRevisions(Guid Id)
        {
            var crispriStrainRevision = await _versionHub.GetVersions(Id);
            return crispriStrainRevision;
        }
    }
}