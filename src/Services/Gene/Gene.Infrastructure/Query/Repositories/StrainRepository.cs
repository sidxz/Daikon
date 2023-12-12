
using CQRS.Core.Exceptions;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Gene.Infrastructure.Query.Repositories
{
    public class StrainRepository : IStrainRepository
    {

        private readonly IMongoCollection<Strain> _strainCollection;
        private readonly ILogger<StrainRepository> _logger;

        public StrainRepository(IConfiguration configuration, ILogger<StrainRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName"));
            _strainCollection = database.GetCollection<Strain>(configuration.GetValue<string>("GeneMongoDbSettings:StrainCollectionName"));
            _strainCollection.Indexes.CreateOne(new CreateIndexModel<Strain>(Builders<Strain>.IndexKeys.Ascending(g => g.Name), new CreateIndexOptions { Unique = true }));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateStrain(Strain strain)
        {

            ArgumentNullException.ThrowIfNull(strain);

            try
            {
                _logger.LogInformation("CreateStrain: Creating strain {strainId}, {strain}", strain.Id, strain.ToJson());
                await _strainCollection.InsertOneAsync(strain);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the strain with ID {strainId}", strain.Id);
                throw new RepositoryException(nameof(StrainRepository), "Error creating strain", ex);
            }
        }



        public async Task<Strain> ReadStrainById(Guid id)
        {
            return await _strainCollection.Find(strain => strain.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Strain> ReadStrainByName(string name)
        {
            return await _strainCollection.Find(strain => strain.Name == name).FirstOrDefaultAsync();
        }


        public async Task<List<Strain>> GetStrainsList()
        {
            try
            {
                return await _strainCollection.Find(strain => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the strain list");
                throw new RepositoryException(nameof(StrainRepository), "Error getting strain list", ex);
            }

        }

        public async Task UpdateStrain(Strain strain)
        {
            ArgumentNullException.ThrowIfNull(strain);

            var filter = Builders<Strain>.Filter.Eq(g => g.Id, strain.Id);
            var update = Builders<Strain>.Update
                .Set(s => s.Name, strain.Name)
                .Set(s => s.Organism, strain.Organism);

            try
            {
                _logger.LogInformation("UpdateStrain: Updating strain {strainId}, {strain}", strain.Id, strain.ToJson());
                await _strainCollection.UpdateOneAsync(filter, update);

            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the strain with ID {strainId}", strain.Id);
                throw new RepositoryException(nameof(StrainRepository), "Error updating strain", ex);
            }

        }


        public async Task DeleteStrain(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                _logger.LogInformation("DeleteStrain: Deleting strain {strainId}", id);
                await _strainCollection.DeleteOneAsync(strain => strain.Id == id);

            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the strain with ID {StrainId}", id);
                throw new RepositoryException(nameof(StrainRepository), "Error deleting strain", ex);
            }

        }

    }
}