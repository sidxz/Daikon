
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
    public class GeneResistanceMutationRepository : IGeneResistanceMutationRepository
    {

        private readonly IMongoCollection<ResistanceMutation> _resistanceMutationCollection;
        private readonly IVersionHub<ResistanceMutationRevision> _versionHub;
        private readonly ILogger<GeneResistanceMutationRepository> _logger;

        public GeneResistanceMutationRepository(IConfiguration configuration, IVersionHub<ResistanceMutationRevision> versionMaintainer, ILogger<GeneResistanceMutationRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName"));
            _resistanceMutationCollection = database.GetCollection<ResistanceMutation>(
                configuration.GetValue<string>("GeneMongoDbSettings:GeneResistanceMutationCollectionName") ??
                configuration.GetValue<string>("GeneMongoDbSettings:GeneCollectionName") + "ResistanceMutation");

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<ResistanceMutation> Read(Guid id)
        {
            try
            {
                return await _resistanceMutationCollection.Find(resistanceMutation => resistanceMutation.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the resistanceMutation with ID {ResistanceMutationId}", id);
                throw new RepositoryException(nameof(GeneResistanceMutationRepository), "Error getting resistanceMutation", ex);
            }
        }

        public async Task<List<ResistanceMutation>> GetResistanceMutationList()
        {
            try
            {
                return await _resistanceMutationCollection.Find(resistanceMutation => true)
                .SortByDescending(resistanceMutation => resistanceMutation.DateCreated)
                .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the resistanceMutation list");
                throw new RepositoryException(nameof(GeneResistanceMutationRepository), "Error getting resistanceMutation list", ex);
            }

        }

        public async Task<List<ResistanceMutation>> GetResistanceMutationOfGene(Guid geneId)
        {
            try
            {
                return await _resistanceMutationCollection.Find(resistanceMutation => resistanceMutation.GeneId == geneId)
                .SortByDescending(resistanceMutation => resistanceMutation.DateCreated)
                .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the resistanceMutation list");
                throw new RepositoryException(nameof(GeneResistanceMutationRepository), "Error getting resistanceMutation list", ex);
            }

        }

        public async Task AddResistanceMutation(ResistanceMutation resistanceMutation)
        {

            ArgumentNullException.ThrowIfNull(resistanceMutation);

            try
            {
                _logger.LogInformation("AddResistanceMutation: Creating ResistanceMutation {ResistanceMutationId}, {resistanceMutation}", resistanceMutation.Id, resistanceMutation.ToJson());
                await _resistanceMutationCollection.InsertOneAsync(resistanceMutation);
                await _versionHub.CommitVersion(resistanceMutation);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the gene with ID {id}", resistanceMutation.Id);
                throw new RepositoryException(nameof(GeneResistanceMutationRepository), "Error creating ResistanceMutation", ex);
            }
        }


        public async Task UpdateResistanceMutation(ResistanceMutation resistanceMutation)
        {
            ArgumentNullException.ThrowIfNull(resistanceMutation);

            try
            {
                _logger.LogInformation("UpdateResistanceMutation: Updating ResistanceMutation {ResistanceMutationId}, {resistanceMutation}", resistanceMutation.Id, resistanceMutation.ToJson());
                await _resistanceMutationCollection.ReplaceOneAsync(r => r.Id == resistanceMutation.Id, resistanceMutation);
                await _versionHub.CommitVersion(resistanceMutation);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the ResistanceMutation with ID {ResistanceMutationId}", resistanceMutation.Id);
                throw new RepositoryException(nameof(GeneResistanceMutationRepository), "Error updating ResistanceMutation", ex);
            }

        }


        public async Task DeleteResistanceMutation(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                _logger.LogInformation("DeleteResistanceMutation: Deleting ResistanceMutation {ResistanceMutation}", id);
                await _resistanceMutationCollection.DeleteOneAsync(gene => gene.Id == id);
                await _versionHub.ArchiveEntity(id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the ResistanceMutation with ID {ResistanceMutation}", id);
                throw new RepositoryException(nameof(GeneResistanceMutationRepository), "Error deleting ResistanceMutation", ex);
            }

        }

        public async Task DeleteAllResistanceMutationsOfGene(Guid geneId)
        {
            ArgumentNullException.ThrowIfNull(geneId);

            // find all resistanceMutations of gene and archive them individually
            var resistanceMutations = await _resistanceMutationCollection.Find(resistanceMutation => resistanceMutation.GeneId == geneId).ToListAsync();
            foreach (var resistanceMutation in resistanceMutations)
            {
                _logger.LogInformation("DeleteResistanceMutationsOfGene: Archiving ResistanceMutation {ResistanceMutationId}", resistanceMutation.Id);
                await _versionHub.ArchiveEntity(resistanceMutation.Id);
            }
            // delete all resistanceMutations of gene
            try
            {
                _logger.LogInformation("DeleteResistanceMutationsOfGene: Deleting ResistanceMutations of Gene {GeneId}", geneId);
                await _resistanceMutationCollection.DeleteManyAsync(resistanceMutation => resistanceMutation.GeneId == geneId);

            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the ResistanceMutations of Gene with ID {GeneId}", geneId);
                throw new RepositoryException(nameof(GeneResistanceMutationRepository), "Error deleting ResistanceMutations of Gene", ex);
            }

        }


        public async Task<ResistanceMutationRevision> GetResistanceMutationRevisions(Guid Id)
        {
            var resistanceMutationRevision = await _versionHub.GetVersions(Id);
            return resistanceMutationRevision;
        }
    }
}