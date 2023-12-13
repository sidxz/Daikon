
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
                _logger.LogError(ex, "An error occurred while creating the gene with ID {GeneId}", resistanceMutation.Id);
                throw new RepositoryException(nameof(GeneResistanceMutationRepository), "Error creating gene", ex);
            }
        }



        public async Task<ResistanceMutation> Read(Guid id)
        {
            return await _resistanceMutationCollection.Find(resistanceMutation => resistanceMutation.Id == id).FirstOrDefaultAsync();
        }




        public async Task<List<ResistanceMutation>> GetResistanceMutationList()
        {
            try
            {
                return await _resistanceMutationCollection.Find(resistanceMutation => true).ToListAsync();
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
                return await _resistanceMutationCollection.Find(resistanceMutation => resistanceMutation.GeneId == geneId).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the resistanceMutation list");
                throw new RepositoryException(nameof(GeneResistanceMutationRepository), "Error getting resistanceMutation list", ex);
            }

        }

        public async Task UpdateResistanceMutation(ResistanceMutation resistanceMutation)
        {
            ArgumentNullException.ThrowIfNull(resistanceMutation);

            var filter = Builders<ResistanceMutation>.Filter.Eq(e => e.Id, resistanceMutation.Id);
            var update = Builders<ResistanceMutation>.Update
                .Set(e => e.Mutation, resistanceMutation.Mutation)
                .Set(e => e.Isolate, resistanceMutation.Isolate)
                .Set(e => e.ParentStrain, resistanceMutation.ParentStrain)
                .Set(e => e.Compound, resistanceMutation.Compound)
                .Set(e => e.ShiftInMIC, resistanceMutation.ShiftInMIC)
                .Set(e => e.Organization, resistanceMutation.Organization)
                .Set(e => e.Researcher, resistanceMutation.Researcher)
                .Set(e => e.Reference, resistanceMutation.Reference)
                .Set(e => e.Notes, resistanceMutation.Notes)

            try
            {
                _logger.LogInformation("UpdateResistanceMutation: Updating resistanceMutation {resistanceMutationId}, {resistanceMutation}", resistanceMutation.Id, resistanceMutation.ToJson());
                await _resistanceMutationCollection.UpdateOneAsync(filter, update);
                await _versionHub.CommitVersion(resistanceMutation);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the resistanceMutation with ID {resistanceMutationId}", resistanceMutation.Id);
                throw new RepositoryException(nameof(GeneResistanceMutationRepository), "Error updating gene", ex);
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