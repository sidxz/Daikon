
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
    public class GeneEssentialityRepository : IGeneEssentialityRepository
    {

        private readonly IMongoCollection<Essentiality> _essentialityCollection;
        private readonly IVersionHub<EssentialityRevision> _versionHub;
        private readonly ILogger<GeneEssentialityRepository> _logger;

        public GeneEssentialityRepository(IConfiguration configuration, IVersionHub<EssentialityRevision> versionMaintainer, ILogger<GeneEssentialityRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName"));
            _essentialityCollection = database.GetCollection<Essentiality>(
                configuration.GetValue<string>("GeneMongoDbSettings:GeneEssentialityCollectionName") ??
                configuration.GetValue<string>("GeneMongoDbSettings:GeneCollectionName") + "Essentiality");

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<Essentiality> Read(Guid id)
        {
            try
            {
                return await _essentialityCollection.Find(essentiality => essentiality.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the essentiality with ID {EssentialityId}", id);
                throw new RepositoryException(nameof(GeneEssentialityRepository), "Error getting essentiality", ex);
            }
        }

        public async Task<List<Essentiality>> GetEssentialityList()
        {
            try
            {
                return await _essentialityCollection.Find(essential => true)
                .SortByDescending(essential => essential.DateCreated)
                .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the essential list");
                throw new RepositoryException(nameof(GeneEssentialityRepository), "Error getting essential list", ex);
            }

        }

        public async Task<List<Essentiality>> GetEssentialityOfGene(Guid geneId)
        {
            try
            {
                return await _essentialityCollection.Find(essentiality => essentiality.GeneId == geneId)
                .SortByDescending(essentiality => essentiality.DateCreated)
                .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the essentiality list");
                throw new RepositoryException(nameof(GeneEssentialityRepository), "Error getting essentiality list", ex);
            }

        }

        public async Task AddEssentiality(Essentiality essentiality)
        {
            _logger.LogInformation("AddEssentiality: Creating Essentiality {EssentialityId}, {essentiality}", essentiality.Id, essentiality.ToJson());

            ArgumentNullException.ThrowIfNull(essentiality);

            try
            {
                await _essentialityCollection.InsertOneAsync(essentiality);
                await _versionHub.CommitVersion(essentiality);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the Essentiality with ID {id}", essentiality.Id);
                throw new RepositoryException(nameof(GeneEssentialityRepository), "Error creating Essentiality", ex);
            }
        }

        public async Task UpdateEssentiality(Essentiality essentiality)
        {
            ArgumentNullException.ThrowIfNull(essentiality);

            _logger.LogInformation("UpdateEssentiality: Updating Essentiality {EssentialityId}, {essentiality}", essentiality.Id, essentiality.ToJson());

            try
            {
                await _essentialityCollection.ReplaceOneAsync(e => e.Id == essentiality.Id, essentiality);
                await _versionHub.CommitVersion(essentiality);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the Essentiality with ID {EssentialityId}", essentiality.Id);
                throw new RepositoryException(nameof(GeneEssentialityRepository), "Error updating Essentiality", ex);
            }

        }


        public async Task DeleteEssentiality(Guid id)
        {
            _logger.LogInformation("DeleteEssentiality: Deleting Essentiality {EssentialityId}", id);

            ArgumentNullException.ThrowIfNull(id);

            try
            {
                await _essentialityCollection.DeleteOneAsync(e => e.Id == id);
                await _versionHub.ArchiveEntity(id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Essentiality with ID {Essentiality}", id);
                throw new RepositoryException(nameof(GeneEssentialityRepository), "Error deleting Essentiality", ex);
            }

        }

        public async Task DeleteAllEssentialitiesOfGene(Guid geneId)
        {
            ArgumentNullException.ThrowIfNull(geneId);

            // find all essentialities of gene and archive them individually
            var essentialities = await _essentialityCollection.Find(essentiality => essentiality.GeneId == geneId).ToListAsync();
            foreach (var essentiality in essentialities)
            {
                _logger.LogInformation("DeleteEssentialitiesOfGene: Archiving Essentiality {EssentialityId}", essentiality.Id);
                await _versionHub.ArchiveEntity(essentiality.Id);
            }
            // delete all essentialities of gene
            try
            {
                _logger.LogInformation("DeleteEssentialitiesOfGene: Deleting Essentialities of Gene {GeneId}", geneId);
                await _essentialityCollection.DeleteManyAsync(essentiality => essentiality.GeneId == geneId);

            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Essentialities of Gene with ID {GeneId}", geneId);
                throw new RepositoryException(nameof(GeneEssentialityRepository), "Error deleting Essentialities of Gene", ex);
            }

        }



        public async Task<EssentialityRevision> GetEssentialityRevisions(Guid Id)
        {
            var essentialityRevision = await _versionHub.GetVersions(Id);
            return essentialityRevision;
        }
    }
}