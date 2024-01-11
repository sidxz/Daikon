
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using HitAssessment.Application.Contracts.Persistence;
using HitAssessment.Domain.EntityRevisions;

namespace HitAssessment.Infrastructure.Query.Repositories
{
    public class HitAssessmentRepository : IHitAssessmentRepository
    {
        private readonly IMongoCollection<Domain.Entities.HitAssessment> _screenCollection;
        private readonly ILogger<HitAssessmentRepository> _logger;
        private readonly IVersionHub<HitAssessmentRevision> _versionHub;

        public HitAssessmentRepository(IConfiguration configuration, ILogger<HitAssessmentRepository> logger, IVersionHub<HitAssessmentRevision> versionMaintainer)
        {
            var client = new MongoClient(configuration.GetValue<string>("HAMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("HAMongoDbSettings:DatabaseName"));
            _screenCollection = database.GetCollection<Domain.Entities.HitAssessment>(configuration.GetValue<string>("HAMongoDbSettings:HitAssessmentCollectionName"));
            _screenCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.HitAssessment>(Builders<Domain.Entities.HitAssessment>.IndexKeys.Ascending(t => t.Name), new CreateIndexOptions { Unique = false }));
            _screenCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.HitAssessment>(Builders<Domain.Entities.HitAssessment>.IndexKeys.Ascending(t => t.StrainId), new CreateIndexOptions { Unique = false }));

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        public async Task CreateHa(Domain.Entities.HitAssessment screen)
        {

            ArgumentNullException.ThrowIfNull(screen);

            try
            {
                _logger.LogInformation("CreateHitAssessment: Creating screen {HitAssessmentId}, {HitAssessment}", screen.Id, screen.ToJson());
                await _screenCollection.InsertOneAsync(screen);
                await _versionHub.CommitVersion(screen);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the screen with ID {HitAssessmentId}", screen.Id);
                throw new RepositoryException(nameof(HitAssessmentRepository), "Error creating screen", ex);
            }
        }


        public async Task<Domain.Entities.HitAssessment> ReadHaById(Guid id)
        {
            return await _screenCollection.Find(screen => screen.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Domain.Entities.HitAssessment> ReadHaByName(string name)
        {
            return await _screenCollection.Find(screen => screen.Name == name).FirstOrDefaultAsync();
        }


        public async Task<List<Domain.Entities.HitAssessment>> GetHaList()
        {
            try
            {
                return await _screenCollection.Find(screen => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the screen list");
                throw new RepositoryException(nameof(HitAssessmentRepository), "Error getting screen list", ex);
            }

        }

        public async Task<List<Domain.Entities.HitAssessment>> GetHaListByStrainId(Guid strainId)
        {
            try
            {
                return await _screenCollection.Find(screen => screen.StrainId == strainId).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the screen list");
                throw new RepositoryException(nameof(HitAssessmentRepository), "Error getting screen list", ex);
            }

        }



        public async Task UpdateHa(Domain.Entities.HitAssessment screen)
        {
            ArgumentNullException.ThrowIfNull(screen);

            try
            {
                _logger.LogInformation("UpdateHitAssessment: Updating screen {HitAssessmentId}, {HitAssessment}", screen.Id, screen.ToJson());
                await _screenCollection.ReplaceOneAsync(t => t.Id == screen.Id, screen);
                await _versionHub.CommitVersion(screen);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the screen with ID {HitAssessmentId}", screen.Id);
                throw new RepositoryException(nameof(HitAssessmentRepository), "Error updating screen", ex);
            }
        }

        public async Task DeleteHa(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                _logger.LogInformation("DeleteHitAssessment: Deleting screen {HitAssessmentId}", id);
                await _screenCollection.DeleteOneAsync(t => t.Id == id);
                await _versionHub.ArchiveEntity(id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the screen with ID {HitAssessmentId}", id);
                throw new RepositoryException(nameof(HitAssessmentRepository), "Error deleting screen", ex);
            }
        }


        public async Task<HitAssessmentRevision> GetHaRevisions(Guid Id)
        {
            var screenRevision = await _versionHub.GetVersions(Id);
            return screenRevision;
        }
    }
}