
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Persistence;
using MLogix.Domain.Entities;
using MongoDB.Driver;

namespace MLogix.Infrastructure.Query.Repositories
{
    public class MoleculeRepository : IMoleculeRepository
    {
        private readonly IMongoCollection<Molecule> _moleculeCollection;
        private readonly ILogger<MoleculeRepository> _logger;


        public MoleculeRepository(IConfiguration configuration, ILogger<MoleculeRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("MLxMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("MLxMongoDbSettings:DatabaseName"));
            _moleculeCollection = database.GetCollection<Molecule>(configuration.GetValue<string>("MLxMongoDbSettings:MoleculeCollectionName"));

            var registrationIdIndexExists = _moleculeCollection.Indexes.List().ToList().Any(index => index["name"] == "RegistrationId_1");
            if (!registrationIdIndexExists)
            {
                _moleculeCollection.Indexes.CreateOne(
                    new CreateIndexModel<Molecule>(
                        Builders<Molecule>.IndexKeys.Ascending(t => t.RegistrationId),
                        new CreateIndexOptions { Unique = true }));
            }

            var nameIndexExists = _moleculeCollection.Indexes.List().ToList().Any(index => index["name"] == "Name_1");
            if (!nameIndexExists)
            {
                _moleculeCollection.Indexes.CreateOne(
                    new CreateIndexModel<Molecule>(
                        Builders<Molecule>.IndexKeys.Ascending(t => t.Name),
                        new CreateIndexOptions { Unique = false, Sparse = true }));
            }

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task NewMolecule(Molecule molecule)
        {
            ArgumentNullException.ThrowIfNull(molecule);
            try
            {
                _logger.LogInformation("NewMolecule: Creating molecule {moleculeId}, {molecule}", molecule.Id, molecule.ToJson());

                await _moleculeCollection.InsertOneAsync(molecule);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the molecule with ID {moleculeId}", molecule.Id);
                throw new RepositoryException(nameof(MoleculeRepository), "Error creating molecule", ex);
            }
        }

        public async Task UpdateMolecule(Molecule molecule)
        {
            ArgumentNullException.ThrowIfNull(molecule);
            try
            {
                _logger.LogInformation("UpdateMolecule: Updating molecule {moleculeId}, {molecule}", molecule.Id, molecule.ToJson());

                await _moleculeCollection.ReplaceOneAsync(m => m.Id == molecule.Id, molecule);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the molecule with ID {moleculeId}", molecule.Id);
                throw new RepositoryException(nameof(MoleculeRepository), "Error updating molecule", ex);
            }
        }

        public async Task DeleteMolecule(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                _logger.LogInformation("DeleteMolecule: Deleting molecule {moleculeId}", id);

                await _moleculeCollection.DeleteOneAsync(m => m.Id == id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the molecule with ID {moleculeId}", id);
                throw new RepositoryException(nameof(MoleculeRepository), "Error deleting molecule", ex);
            }
        }

        public async Task<Molecule> GetMoleculeById(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                _logger.LogInformation("GetMoleculeById: Fetching molecule {moleculeId}", id);

                return await _moleculeCollection.Find(m => m.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the molecule with ID {moleculeId}", id);
                throw new RepositoryException(nameof(MoleculeRepository), "Error fetching molecule", ex);
            }
        }

        public async Task<List<Molecule>> GetMoleculesByIds(List<Guid> ids)
        {
            ArgumentNullException.ThrowIfNull(ids);
            try
            {
                _logger.LogInformation("GetMoleculesByIds: Fetching molecules {moleculeIds}", ids);

                return await _moleculeCollection.Find(m => ids.Contains(m.Id)).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the molecules with IDs {moleculeIds}", ids);
                throw new RepositoryException(nameof(MoleculeRepository), "Error fetching molecules", ex);
            }
        }

        public async Task<Molecule> GetMoleculeByRegistrationId(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                _logger.LogInformation("GetMoleculeByRegistrationId: Fetching molecule {moleculeId}", id);

                return await _moleculeCollection.Find(m => m.RegistrationId == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the molecule with Registration ID {moleculeId}", id);
                throw new RepositoryException(nameof(MoleculeRepository), "Error fetching molecule", ex);
            }
        }

        public async Task<Molecule> GetByName(string name)
        {
            ArgumentNullException.ThrowIfNull(name);
            try
            {
                _logger.LogInformation("GetByName: Fetching molecule {moleculeName}", name);

                return await _moleculeCollection.Find(m => m.Name.Value == name).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the molecule with name {moleculeName}", name);
                throw new RepositoryException(nameof(MoleculeRepository), "Error fetching molecule", ex);
            }
        }

        public Task<List<Molecule>> GetAllMolecules()
        {
            try
            {
                _logger.LogInformation("GetAllMolecules: Fetching all molecules");

                return _moleculeCollection.Find(m => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all molecules");
                throw new RepositoryException(nameof(MoleculeRepository), "Error fetching all molecules", ex);
            }
        }
    }
}