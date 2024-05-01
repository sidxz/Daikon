
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Entities;
using Screen.Domain.EntityRevisions;

namespace Screen.Infrastructure.Query.Repositories
{
    public class ScreenRunRepository : IScreenRunRepository
    {
        private readonly IMongoCollection<ScreenRun> _screenRun;
        private readonly ILogger<ScreenRunRepository> _logger;
        private readonly IVersionHub<ScreenRunRevision> _versionHub;


        public ScreenRunRepository(IConfiguration configuration, ILogger<ScreenRunRepository> logger, IVersionHub<ScreenRunRevision> versionMaintainer)
        {
            var client = new MongoClient(configuration.GetValue<string>("ScreenMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("ScreenMongoDbSettings:DatabaseName"));
            _screenRun = database.GetCollection<ScreenRun>(configuration.GetValue<string>("ScreenMongoDbSettings:ScreenRunCollectionName"));
            _screenRun.Indexes.CreateOne(new CreateIndexModel<ScreenRun>(Builders<ScreenRun>.IndexKeys.Ascending(t => t.ScreenId), new CreateIndexOptions { Unique = false }));

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task CreateScreenRun(ScreenRun screenRun)
        {
            ArgumentNullException.ThrowIfNull(screenRun);
            try
            {
                _logger.LogInformation("CreateScreenRun: Creating screenRun {ScreenRunId}, {ScreenRun}", screenRun.Id, screenRun.ToJson());
                await _screenRun.InsertOneAsync(screenRun);
                await _versionHub.CommitVersion(screenRun);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the screenRun with ID {ScreenRunId}", screenRun.Id);
                throw new RepositoryException(nameof(ScreenRunRepository), "Error creating screenRun", ex);
            }
        }

        public Task DeleteScreenRun(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                _logger.LogInformation("DeleteScreenRun: Deleting screenRun {ScreenRunId}", id);
                var result = _screenRun.DeleteOneAsync(screenRun => screenRun.Id == id);
                return result;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the screenRun with ID {ScreenRunId}", id);
                throw new RepositoryException(nameof(ScreenRunRepository), "Error deleting screenRun", ex);
            }
        }

        public Task DeleteScreenRunsByScreenId(Guid screenId)
        {
            ArgumentNullException.ThrowIfNull(screenId);
            try
            {
                _logger.LogInformation("DeleteScreenRunsByScreenId: Deleting screenRuns {ScreenId}", screenId);
                var result = _screenRun.DeleteManyAsync(screenRun => screenRun.ScreenId == screenId);
                return result;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the screenRuns with ScreenId {ScreenId}", screenId);
                throw new RepositoryException(nameof(ScreenRunRepository), "Error deleting screenRuns", ex);
            }
        }

        public Task<List<ScreenRun>> GetScreenRunList()
        {
            try
            {
                return _screenRun.Find(screenRun => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the screenRun list");
                throw new RepositoryException(nameof(ScreenRunRepository), "Error getting screenRun list", ex);
            }
        }

        public Task<List<ScreenRun>> GetScreenRunsListByScreenId(Guid screenId)
        {
            ArgumentNullException.ThrowIfNull(screenId);
            try
            {
                return _screenRun.Find(screenRun => screenRun.ScreenId == screenId).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the screenRun list");
                throw new RepositoryException(nameof(ScreenRunRepository), "Error getting screenRun list", ex);
            }
        }

        public async Task<ScreenRunRevision> GetScreenRunRevisions(Guid Id)
        {
            var screenRunRevision = await _versionHub.GetVersions(Id);
            return screenRunRevision;
        }

        public Task<ScreenRun> ReadScreenRunById(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                return _screenRun.Find(screenRun => screenRun.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the screenRun with ID {ScreenRunId}", id);
                throw new RepositoryException(nameof(ScreenRunRepository), "Error getting screenRun", ex);
            }
        }

        public Task UpdateScreenRun(ScreenRun screenRun)
        {
            ArgumentNullException.ThrowIfNull(screenRun);
            try
            {
                _logger.LogInformation("UpdateScreenRun: Updating screenRun {ScreenRunId}, {ScreenRun}", screenRun.Id, System.Text.Json.JsonSerializer.Serialize(screenRun));
                var result = _screenRun.ReplaceOneAsync(sr => sr.Id == screenRun.Id, screenRun);
                return result;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the screenRun with ID {ScreenRunId}", screenRun.Id);
                throw new RepositoryException(nameof(ScreenRunRepository), "Error updating screenRun", ex);
            }
        }
    }
}