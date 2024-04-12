
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

    

        public async Task<Strain> ReadStrainById(Guid id)
        {
            try
            {
                return await _strainCollection.Find(strain => strain.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the strain with ID {StrainId}", id);
                throw new RepositoryException(nameof(StrainRepository), "Error getting strain", ex);
            }
        }

        public async Task<Strain> ReadStrainByName(string name)
        {
            try
            {
                return await _strainCollection.Find(strain => strain.Name == name).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the strain with name {StrainName}", name);
                throw new RepositoryException(nameof(StrainRepository), "Error getting strain", ex);
            }
        }


        public async Task<List<Strain>> GetStrainsList()
        {
            try
            {
                return await _strainCollection.Find(strain => true)
                .SortBy(strain => strain.Name)
                .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the strain list");
                throw new RepositoryException(nameof(StrainRepository), "Error getting strain list", ex);
            }

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

        public async Task UpdateStrain(Strain strain)
        {
            ArgumentNullException.ThrowIfNull(strain);

            try
            {
                _logger.LogInformation("UpdateStrain: Updating strain {strainId}, {strain}", strain.Id, strain.ToJson());
                await _strainCollection.ReplaceOneAsync(s => s.Id == strain.Id, strain);
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