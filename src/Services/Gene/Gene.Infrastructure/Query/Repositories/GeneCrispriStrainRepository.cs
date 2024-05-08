
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

            _crispriStrainCollection.Indexes.CreateOne
                (new CreateIndexModel<CrispriStrain>(Builders<CrispriStrain>.IndexKeys.Ascending(t => t.DateCreated), new CreateIndexOptions { Unique = false }));
            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<CrispriStrain> Read(Guid id)
        {
            try
            {
                return await _crispriStrainCollection.Find(crispriStrain => crispriStrain.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the crispriStrain with ID {CrispriStrainId}", id);
                throw new RepositoryException(nameof(GeneCrispriStrainRepository), "Error getting crispriStrain", ex);
            }
        }

        public async Task<List<CrispriStrain>> GetCrispriStrainList()
        {
            try
            {
                return await _crispriStrainCollection.Find(crispriStrain => true)
                    .SortBy(crispriStrain => crispriStrain.DateCreated)
                    .ToListAsync();
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
                return await _crispriStrainCollection.Find(crispriStrain => crispriStrain.GeneId == geneId)
                .SortBy(crispriStrain => crispriStrain.DateCreated)
                .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the crispriStrain list");
                throw new RepositoryException(nameof(GeneCrispriStrainRepository), "Error getting crispriStrain list", ex);
            }

        }

        public async Task AddCrispriStrain(CrispriStrain crispriStrain)
        {

            ArgumentNullException.ThrowIfNull(crispriStrain);
            _logger.LogInformation("AddCrispriStrain: Creating CrispriStrain {CrispriStrainId}, {crispriStrain}", crispriStrain.Id, crispriStrain.ToJson());

            try
            {
                await _crispriStrainCollection.InsertOneAsync(crispriStrain);
                await _versionHub.CommitVersion(crispriStrain);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the CrispriStrain with ID {id}", crispriStrain.Id);
                throw new RepositoryException(nameof(GeneCrispriStrainRepository), "Error creating CrispriStrain", ex);
            }
        }

        public async Task UpdateCrispriStrain(CrispriStrain crispriStrain)
        {
            ArgumentNullException.ThrowIfNull(crispriStrain);

            _logger.LogInformation("UpdateCrispriStrain: Updating CrispriStrain {CrispriStrainId}, {crispriStrain}", crispriStrain.Id, crispriStrain.ToJson());

            try
            {
                await _crispriStrainCollection.ReplaceOneAsync(crispr => crispr.Id == crispriStrain.Id, crispriStrain);
                await _versionHub.CommitVersion(crispriStrain);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the CrispriStrain with ID {CrispriStrain}", crispriStrain.Id);
                throw new RepositoryException(nameof(GeneCrispriStrainRepository), "Error updating CrispriStrain", ex);
            }

        }


        public async Task DeleteCrispriStrain(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                _logger.LogInformation("DeleteCrispriStrain: Deleting CrispriStrain {CrispriStrain}", id);
                await _crispriStrainCollection.DeleteOneAsync(crispr => crispr.Id == id);
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