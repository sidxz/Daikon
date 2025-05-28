
using CQRS.Core.Exceptions;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Gene.Infrastructure.Query.Repositories
{
    public class GeneHypomorphRepository : IGeneHypomorphRepository
    {

        private readonly IMongoCollection<Hypomorph> _hypomorphCollection;
        private readonly ILogger<GeneHypomorphRepository> _logger;

        public GeneHypomorphRepository(IConfiguration configuration, ILogger<GeneHypomorphRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName"));
            _hypomorphCollection = database.GetCollection<Hypomorph>(
                configuration.GetValue<string>("GeneMongoDbSettings:GeneHypomorphCollectionName") ??
                configuration.GetValue<string>("GeneMongoDbSettings:GeneCollectionName") + "Hypomorph");

            _hypomorphCollection.Indexes.CreateOne
                (new CreateIndexModel<Hypomorph>(Builders<Hypomorph>.IndexKeys.Ascending(t => t.DateCreated), new CreateIndexOptions { Unique = false }));
                
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        public async Task<Hypomorph> Read(Guid id)
        {
            try
            {
                return await _hypomorphCollection.Find(hypomorph => hypomorph.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the hypomorph with ID {HypomorphId}", id);
                throw new RepositoryException(nameof(GeneHypomorphRepository), "Error getting hypomorph", ex);
            }
        }


        public async Task<List<Hypomorph>> GetHypomorphList()
        {
            try
            {
                return await _hypomorphCollection.Find(hypomorph => true)
                .SortBy(hypomorph => hypomorph.DateCreated)
                .ToListAsync();
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
                return await _hypomorphCollection.Find(hypomorph => hypomorph.GeneId == geneId)
                .SortBy(hypomorph => hypomorph.DateCreated)
                .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the hypomorph list");
                throw new RepositoryException(nameof(GeneHypomorphRepository), "Error getting hypomorph list", ex);
            }

        }

        public async Task AddHypomorph(Hypomorph hypomorph)
        {
            _logger.LogInformation("AddHypomorph: Creating Hypomorph {HypomorphId}, {hypomorph}", hypomorph.Id, hypomorph.ToJson());
            ArgumentNullException.ThrowIfNull(hypomorph);

            try
            {
                await _hypomorphCollection.InsertOneAsync(hypomorph);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the Hypomorph with ID {id}", hypomorph.Id);
                throw new RepositoryException(nameof(GeneHypomorphRepository), "Error creating Hypomorph", ex);
            }
        }

        public async Task UpdateHypomorph(Hypomorph hypomorph)
        {
            _logger.LogInformation("UpdateHypomorph: Updating Hypomorph {HypomorphId}, {hypomorph}", hypomorph.Id, hypomorph.ToJson());
            ArgumentNullException.ThrowIfNull(hypomorph);

            try
            {
                await _hypomorphCollection.ReplaceOneAsync(h => h.Id == hypomorph.Id, hypomorph);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the Hypomorph with ID {HypomorphId}", hypomorph.Id);
                throw new RepositoryException(nameof(GeneHypomorphRepository), "Error updating Hypomorph", ex);
            }
        }

        public async Task DeleteHypomorph(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                _logger.LogInformation("DeleteHypomorph: Deleting Hypomorph {Hypomorph}", id);
                await _hypomorphCollection.DeleteOneAsync(h => h.Id == id);
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
    }
}