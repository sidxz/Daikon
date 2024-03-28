
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using HitAssessment.Application.Contracts.Persistence;
using HitAssessment.Domain.Entities;
using HitAssessment.Domain.EntityRevisions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace HitAssessment.Infrastructure.Query.Repositories
{
    public class HaCompoundEvolutionRepository : IHaCompoundEvolutionRepository
    {

        private readonly IMongoCollection<HaCompoundEvolution> _haCompoundEvoCollection;
        private readonly ILogger<HaCompoundEvolutionRepository> _logger;
        private readonly IVersionHub<HaCompoundEvolutionRevision> _versionHub;

        public HaCompoundEvolutionRepository(IConfiguration configuration, ILogger<HaCompoundEvolutionRepository> logger, IVersionHub<HaCompoundEvolutionRevision> versionMaintainer)
        {
            var client = new MongoClient(configuration.GetValue<string>("HAMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("HAMongoDbSettings:DatabaseName"));
            _haCompoundEvoCollection = database.GetCollection<HaCompoundEvolution>(configuration.GetValue<string>("HAMongoDbSettings:HaCompoundEvolutionCollectionName"));
            _haCompoundEvoCollection.Indexes.CreateOne(new CreateIndexModel<HaCompoundEvolution>(Builders<HaCompoundEvolution>.IndexKeys.Ascending(t => t.HitAssessmentId), new CreateIndexOptions { Unique = false }));

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateHaCompoundEvolution(HaCompoundEvolution haCompoundEvolution)
        {
            ArgumentNullException.ThrowIfNull(haCompoundEvolution);

            try
            {
                _logger.LogInformation("CreateHaCompoundEvolution: Creating haCompoundEvolution {HaCompoundEvolutionId}, {HaCompoundEvolution}", haCompoundEvolution.Id, haCompoundEvolution.ToJson());
                await _haCompoundEvoCollection.InsertOneAsync(haCompoundEvolution);
                await _versionHub.CommitVersion(haCompoundEvolution);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the haCompoundEvolution with ID {HaCompoundEvolutionId}", haCompoundEvolution.Id);
                throw new RepositoryException(nameof(HaCompoundEvolutionRepository), "Error creating haCompoundEvolution", ex);
            }
        }

        public async Task DeleteHaCompoundEvolution(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                _logger.LogInformation("DeleteHaCompoundEvolution: Deleting haCompoundEvolution {HaCompoundEvolutionId}", id);
                await _haCompoundEvoCollection.DeleteOneAsync(haCompoundEvolution => haCompoundEvolution.Id == id);
                await _versionHub.ArchiveEntity(id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the haCompoundEvolution with ID {HaCompoundEvolutionId}", id);
                throw new RepositoryException(nameof(HaCompoundEvolutionRepository), "Error deleting haCompoundEvolution", ex);
            }
        }

        public async Task<List<HaCompoundEvolution>> GetHaCompoundEvolutionOfHa(Guid HaId)
        {
            ArgumentNullException.ThrowIfNull(HaId);
            try
            {
                _logger.LogInformation("GetHaCompoundEvolutionOfHa: Getting haCompoundEvolution of ha {HaId}", HaId);
                // sort by created date EvolutionDate.Value

                return await _haCompoundEvoCollection.Find(haCompoundEvolution => haCompoundEvolution.HitAssessmentId == HaId)
                .SortByDescending(haCompoundEvolution => haCompoundEvolution.EvolutionDate.Value)
                .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the haCompoundEvolution of ha with ID {HaId}", HaId);
                throw new RepositoryException(nameof(HaCompoundEvolutionRepository), "Error getting haCompoundEvolution of ha", ex);
            }

        }



        public async Task<HaCompoundEvolution> ReadHaCompoundEvolutionById(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                _logger.LogInformation("ReadHaCompoundEvolutionById: Reading haCompoundEvolution {HaCompoundEvolutionId}", id);
                return await _haCompoundEvoCollection.Find(haCompoundEvolution => haCompoundEvolution.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while reading the haCompoundEvolution with ID {HaCompoundEvolutionId}", id);
                throw new RepositoryException(nameof(HaCompoundEvolutionRepository), "Error reading haCompoundEvolution", ex);
            }
        }

        public async Task UpdateHaCompoundEvolution(HaCompoundEvolution haCompoundEvolution)
        {
            ArgumentNullException.ThrowIfNull(haCompoundEvolution);
            try
            {
                _logger.LogInformation("UpdateHaCompoundEvolution: Updating haCompoundEvolution {HaCompoundEvolutionId}, {HaCompoundEvolution}", haCompoundEvolution.Id, haCompoundEvolution.ToJson());
                await _haCompoundEvoCollection.ReplaceOneAsync(t => t.Id == haCompoundEvolution.Id, haCompoundEvolution);
                await _versionHub.CommitVersion(haCompoundEvolution);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the haCompoundEvolution with ID {HaCompoundEvolutionId}", haCompoundEvolution.Id);
                throw new RepositoryException(nameof(HaCompoundEvolutionRepository), "Error updating haCompoundEvolution", ex);
            }
        }


        public async Task<HaCompoundEvolutionRevision> GetHaCompoundEvolutionRevisions(Guid Id)
        {
            var haCompoundEvolutionRevision = await _versionHub.GetVersions(Id);
            return haCompoundEvolutionRevision;
        }
    }
}