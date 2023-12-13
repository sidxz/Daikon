
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
    public class GeneUnpublishedStructuralInformationRepository : IGeneUnpublishedStructuralInformationRepository
    {

        private readonly IMongoCollection<UnpublishedStructuralInformation> _unpublishedStructuralInformationCollection; 
        private readonly IVersionHub<UnpublishedStructuralInformationRevision> _versionHub;
        private readonly ILogger<GeneUnpublishedStructuralInformationRepository> _logger;

        public GeneUnpublishedStructuralInformationRepository(IConfiguration configuration, IVersionHub<UnpublishedStructuralInformationRevision> versionMaintainer, ILogger<GeneUnpublishedStructuralInformationRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName"));
            _unpublishedStructuralInformationCollection = database.GetCollection<UnpublishedStructuralInformation>(
                configuration.GetValue<string>("GeneMongoDbSettings:GeneUnpublishedStructuralInformationCollectionName") ?? 
                configuration.GetValue<string>("GeneMongoDbSettings:GeneCollectionName") + "UnpublishedStructuralInformation");

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddUnpublishedStructuralInformation(UnpublishedStructuralInformation unpublishedStructuralInformation)
        {

            ArgumentNullException.ThrowIfNull(unpublishedStructuralInformation);

            try
            {
                _logger.LogInformation("AddUnpublishedStructuralInformation: Creating UnpublishedStructuralInformation {UnpublishedStructuralInformationId}, {unpublishedStructuralInformation}", unpublishedStructuralInformation.Id, unpublishedStructuralInformation.ToJson());
                await _unpublishedStructuralInformationCollection.InsertOneAsync(unpublishedStructuralInformation);
                await _versionHub.CommitVersion(unpublishedStructuralInformation);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the gene with ID {GeneId}", unpublishedStructuralInformation.Id);
                throw new RepositoryException(nameof(GeneUnpublishedStructuralInformationRepository), "Error creating gene", ex);
            }
        }



        public async Task<UnpublishedStructuralInformation> Read(Guid id)
        {
            return await _unpublishedStructuralInformationCollection.Find(unpublishedStructuralInformation => unpublishedStructuralInformation.Id == id).FirstOrDefaultAsync();
        }




        public async Task<List<UnpublishedStructuralInformation>> GetUnpublishedStructuralInformationList()
        {
            try
            {
                return await _unpublishedStructuralInformationCollection.Find(unpublishedStructuralInformation => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the unpublishedStructuralInformation list");
                throw new RepositoryException(nameof(GeneUnpublishedStructuralInformationRepository), "Error getting unpublishedStructuralInformation list", ex);
            }

        }

        public async Task<List<UnpublishedStructuralInformation>> GetUnpublishedStructuralInformationOfGene(Guid geneId)
        {
            try
            {
                return await _unpublishedStructuralInformationCollection.Find(unpublishedStructuralInformation => unpublishedStructuralInformation.GeneId == geneId).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the unpublishedStructuralInformation list");
                throw new RepositoryException(nameof(GeneUnpublishedStructuralInformationRepository), "Error getting unpublishedStructuralInformation list", ex);
            }

        }

        public async Task UpdateUnpublishedStructuralInformation(UnpublishedStructuralInformation unpublishedStructuralInformation)
        {
            ArgumentNullException.ThrowIfNull(unpublishedStructuralInformation);

            var filter = Builders<UnpublishedStructuralInformation>.Filter.Eq(e => e.Id, unpublishedStructuralInformation.Id);
            var update = Builders<UnpublishedStructuralInformation>.Update
                .Set(e => e.Organization, unpublishedStructuralInformation.Organization)
                .Set(e => e.Method, unpublishedStructuralInformation.Method)
                .Set(e => e.Resolution, unpublishedStructuralInformation.Resolution)
                .Set(e => e.Ligands, unpublishedStructuralInformation.Ligands)
                .Set(e => e.Researcher, unpublishedStructuralInformation.Researcher)
                .Set(e => e.Reference, unpublishedStructuralInformation.Reference)
                .Set(e => e.Notes, unpublishedStructuralInformation.Notes)
                .Set(e => e.URL, unpublishedStructuralInformation.URL);
            try
            {
                _logger.LogInformation("UpdateUnpublishedStructuralInformation: Updating unpublishedStructuralInformation {unpublishedStructuralInformationId}, {unpublishedStructuralInformation}", unpublishedStructuralInformation.Id, unpublishedStructuralInformation.ToJson());
                await _unpublishedStructuralInformationCollection.UpdateOneAsync(filter, update);
                await _versionHub.CommitVersion(unpublishedStructuralInformation);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the unpublishedStructuralInformation with ID {unpublishedStructuralInformationId}", unpublishedStructuralInformation.Id);
                throw new RepositoryException(nameof(GeneUnpublishedStructuralInformationRepository), "Error updating gene", ex);
            }

        }


        public async Task DeleteUnpublishedStructuralInformation(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                _logger.LogInformation("DeleteUnpublishedStructuralInformation: Deleting UnpublishedStructuralInformation {UnpublishedStructuralInformation}", id);
                await _unpublishedStructuralInformationCollection.DeleteOneAsync(gene => gene.Id == id);
                await _versionHub.ArchiveEntity(id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the UnpublishedStructuralInformation with ID {UnpublishedStructuralInformation}", id);
                throw new RepositoryException(nameof(GeneUnpublishedStructuralInformationRepository), "Error deleting UnpublishedStructuralInformation", ex);
            }

        }

        public async Task DeleteAllUnpublishedStructuralInformationsOfGene(Guid geneId)
        {
            ArgumentNullException.ThrowIfNull(geneId);

            // find all unpublishedStructuralInformations of gene and archive them individually
            var unpublishedStructuralInformations = await _unpublishedStructuralInformationCollection.Find(unpublishedStructuralInformation => unpublishedStructuralInformation.GeneId == geneId).ToListAsync();
            foreach (var unpublishedStructuralInformation in unpublishedStructuralInformations)
            {
                _logger.LogInformation("DeleteUnpublishedStructuralInformationsOfGene: Archiving UnpublishedStructuralInformation {UnpublishedStructuralInformationId}", unpublishedStructuralInformation.Id);
                await _versionHub.ArchiveEntity(unpublishedStructuralInformation.Id);
            }
            // delete all unpublishedStructuralInformations of gene
            try
            {
                _logger.LogInformation("DeleteUnpublishedStructuralInformationsOfGene: Deleting UnpublishedStructuralInformations of Gene {GeneId}", geneId);
                await _unpublishedStructuralInformationCollection.DeleteManyAsync(unpublishedStructuralInformation => unpublishedStructuralInformation.GeneId == geneId);
                
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the UnpublishedStructuralInformations of Gene with ID {GeneId}", geneId);
                throw new RepositoryException(nameof(GeneUnpublishedStructuralInformationRepository), "Error deleting UnpublishedStructuralInformations of Gene", ex);
            }

        }



        public async Task<UnpublishedStructuralInformationRevision> GetUnpublishedStructuralInformationRevisions(Guid Id)
        {
            var unpublishedStructuralInformationRevision = await _versionHub.GetVersions(Id);
            return unpublishedStructuralInformationRevision;
        }
    }
}