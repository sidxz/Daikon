
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
    public class GeneHypomorphRepository : IGeneHypomorphRepository
    {

        private readonly IMongoCollection<Hypomorph> _hypomorphCollection; 
        private readonly IVersionHub<HypomorphRevision> _versionHub;
        private readonly ILogger<GeneHypomorphRepository> _logger;

        public GeneHypomorphRepository(IConfiguration configuration, IVersionHub<HypomorphRevision> versionMaintainer, ILogger<GeneHypomorphRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName"));
            _hypomorphCollection = database.GetCollection<Hypomorph>(
                configuration.GetValue<string>("GeneMongoDbSettings:GeneHypomorphCollectionName") ?? 
                configuration.GetValue<string>("GeneMongoDbSettings:GeneCollectionName") + "Hypomorph");

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddHypomorph(Hypomorph hypomorph)
        {

            ArgumentNullException.ThrowIfNull(hypomorph);

            try
            {
                _logger.LogInformation("AddHypomorph: Creating Hypomorph {HypomorphId}, {hypomorph}", hypomorph.Id, hypomorph.ToJson());
                await _hypomorphCollection.InsertOneAsync(hypomorph);
                await _versionHub.CommitVersion(hypomorph);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the gene with ID {GeneId}", hypomorph.Id);
                throw new RepositoryException(nameof(GeneHypomorphRepository), "Error creating gene", ex);
            }
        }



        public async Task<Hypomorph> Read(Guid id)
        {
            return await _hypomorphCollection.Find(hypomorph => hypomorph.Id == id).FirstOrDefaultAsync();
        }




        public async Task<List<Hypomorph>> GetHypomorphList()
        {
            try
            {
                return await _hypomorphCollection.Find(hypomorph => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the hypomorph list");
                throw new RepositoryException(nameof(GeneHypomorphRepository), "Error getting hypomorph list", ex);
            }

        }

        public async Task<List<Hypomorph>> GetHypomorphOfGene(Guid geneId)
        {
            try
            {
                return await _hypomorphCollection.Find(hypomorph => hypomorph.GeneId == geneId).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the hypomorph list");
                throw new RepositoryException(nameof(GeneHypomorphRepository), "Error getting hypomorph list", ex);
            }

        }

        public async Task UpdateHypomorph(Hypomorph hypomorph)
        {
            ArgumentNullException.ThrowIfNull(hypomorph);

            var filter = Builders<Hypomorph>.Filter.Eq(e => e.Id, hypomorph.Id);
            var update = Builders<Hypomorph>.Update
                .Set(e => e.KnockdownStrain, hypomorph.KnockdownStrain)
                .Set(e => e.Phenotype, hypomorph.Phenotype)
                .Set(e => e.Notes, hypomorph.Notes);
            try
            {
                _logger.LogInformation("UpdateHypomorph: Updating hypomorph {hypomorphId}, {hypomorph}", hypomorph.Id, hypomorph.ToJson());
                await _hypomorphCollection.UpdateOneAsync(filter, update);
                await _versionHub.CommitVersion(hypomorph);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the hypomorph with ID {hypomorphId}", hypomorph.Id);
                throw new RepositoryException(nameof(GeneHypomorphRepository), "Error updating gene", ex);
            }

        }


        public async Task DeleteHypomorph(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                _logger.LogInformation("DeleteHypomorph: Deleting Hypomorph {Hypomorph}", id);
                await _hypomorphCollection.DeleteOneAsync(gene => gene.Id == id);
                await _versionHub.ArchiveEntity(id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Hypomorph with ID {Hypomorph}", id);
                throw new RepositoryException(nameof(GeneHypomorphRepository), "Error deleting Hypomorph", ex);
            }

        }

        public async Task DeleteAllHypomorphsOfGene(Guid geneId)
        {
            ArgumentNullException.ThrowIfNull(geneId);

            // find all hypomorphs of gene and archive them individually
            var hypomorphs = await _hypomorphCollection.Find(hypomorph => hypomorph.GeneId == geneId).ToListAsync();
            foreach (var hypomorph in hypomorphs)
            {
                _logger.LogInformation("DeleteHypomorphsOfGene: Archiving Hypomorph {HypomorphId}", hypomorph.Id);
                await _versionHub.ArchiveEntity(hypomorph.Id);
            }
            // delete all hypomorphs of gene
            try
            {
                _logger.LogInformation("DeleteHypomorphsOfGene: Deleting Hypomorphs of Gene {GeneId}", geneId);
                await _hypomorphCollection.DeleteManyAsync(hypomorph => hypomorph.GeneId == geneId);
                
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Hypomorphs of Gene with ID {GeneId}", geneId);
                throw new RepositoryException(nameof(GeneHypomorphRepository), "Error deleting Hypomorphs of Gene", ex);
            }

        }



        public async Task<HypomorphRevision> GetHypomorphRevisions(Guid Id)
        {
            var hypomorphRevision = await _versionHub.GetVersions(Id);
            return hypomorphRevision;
        }
    }
}