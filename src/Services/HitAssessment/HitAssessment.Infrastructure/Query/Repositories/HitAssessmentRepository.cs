
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using HitAssessment.Application.Contracts.Persistence;

namespace HitAssessment.Infrastructure.Query.Repositories
{
    public class HitAssessmentRepository : IHitAssessmentRepository
    {
        private readonly IMongoCollection<Domain.Entities.HitAssessment> _haCollection;
        private readonly ILogger<HitAssessmentRepository> _logger;

        public HitAssessmentRepository(IConfiguration configuration, ILogger<HitAssessmentRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("HAMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("HAMongoDbSettings:DatabaseName"));
            _haCollection = database.GetCollection<Domain.Entities.HitAssessment>(configuration.GetValue<string>("HAMongoDbSettings:HitAssessmentCollectionName"));
            _haCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.HitAssessment>(Builders<Domain.Entities.HitAssessment>.IndexKeys.Ascending(t => t.Name), new CreateIndexOptions { Unique = false }));
            _haCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.HitAssessment>(Builders<Domain.Entities.HitAssessment>.IndexKeys.Ascending(t => t.StrainId), new CreateIndexOptions { Unique = false }));
            _haCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.HitAssessment>(Builders<Domain.Entities.HitAssessment>.IndexKeys.Descending(t => t.DateCreated), new CreateIndexOptions { Unique = false }));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateHa(Domain.Entities.HitAssessment ha)
        {

            ArgumentNullException.ThrowIfNull(ha);

            try
            {
                _logger.LogInformation("CreateHitAssessment: Creating ha {HitAssessmentId}, {HitAssessment}", ha.Id, ha.ToJson());
                await _haCollection.InsertOneAsync(ha);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the ha with ID {HitAssessmentId}", ha.Id);
                throw new RepositoryException(nameof(HitAssessmentRepository), "Error creating ha", ex);
            }
        }


        public async Task<Domain.Entities.HitAssessment> ReadHaById(Guid id)
        {
            return await _haCollection.Find(ha => ha.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Domain.Entities.HitAssessment> ReadHaByName(string name)
        {
            return await _haCollection.Find(ha => ha.Name == name).FirstOrDefaultAsync();
        }


        public async Task<List<Domain.Entities.HitAssessment>> GetHaList()
        {
            try
            {
                return await _haCollection.Find(ha => true)
                .SortBy(ha => ha.Name)
                .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the ha list");
                throw new RepositoryException(nameof(HitAssessmentRepository), "Error getting ha list", ex);
            }

        }

        public async Task<List<Domain.Entities.HitAssessment>> GetHaListByStrainId(Guid strainId)
        {
            try
            {
                return await _haCollection.Find(ha => ha.StrainId == strainId).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the ha list");
                throw new RepositoryException(nameof(HitAssessmentRepository), "Error getting ha list", ex);
            }

        }



        public async Task UpdateHa(Domain.Entities.HitAssessment ha)
        {
            ArgumentNullException.ThrowIfNull(ha);

            try
            {
                _logger.LogInformation("UpdateHitAssessment: Updating ha {HitAssessmentId}, {HitAssessment}", ha.Id, ha.ToJson());
                await _haCollection.ReplaceOneAsync(t => t.Id == ha.Id, ha);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the ha with ID {HitAssessmentId}", ha.Id);
                throw new RepositoryException(nameof(HitAssessmentRepository), "Error updating ha", ex);
            }
        }

        public async Task DeleteHa(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                _logger.LogInformation("DeleteHitAssessment: Deleting ha {HitAssessmentId}", id);
                await _haCollection.DeleteOneAsync(t => t.Id == id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the ha with ID {HitAssessmentId}", id);
                throw new RepositoryException(nameof(HitAssessmentRepository), "Error deleting ha", ex);
            }
        }
    }
}