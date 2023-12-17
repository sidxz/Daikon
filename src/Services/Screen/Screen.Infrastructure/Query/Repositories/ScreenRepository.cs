
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.EntityRevisions;

namespace Screen.Infrastructure.Query.Repositories
{
    public class ScreenRepository : IScreenRepository
    {
        private readonly IMongoCollection<Domain.Entities.Screen> _screenCollection;
        private readonly ILogger<ScreenRepository> _logger;
        private readonly IVersionHub<ScreenRevision> _versionHub;

        public ScreenRepository(IConfiguration configuration, ILogger<ScreenRepository> logger, IVersionHub<ScreenRevision> versionMaintainer)
        {
            var client = new MongoClient(configuration.GetValue<string>("ScreenMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("ScreenMongoDbSettings:DatabaseName"));
            _screenCollection = database.GetCollection<Domain.Entities.Screen>(configuration.GetValue<string>("ScreenMongoDbSettings:ScreenCollectionName"));
            _screenCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Screen>(Builders<Domain.Entities.Screen>.IndexKeys.Ascending(t => t.Name), new CreateIndexOptions { Unique = false }));
            _screenCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Screen>(Builders<Domain.Entities.Screen>.IndexKeys.Ascending(t => t.StrainId), new CreateIndexOptions { Unique = false }));

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        public async Task CreateScreen(Domain.Entities.Screen screen)
        {

            ArgumentNullException.ThrowIfNull(screen);

            try
            {
                _logger.LogInformation("CreateScreen: Creating screen {ScreenId}, {Screen}", screen.Id, screen.ToJson());
                await _screenCollection.InsertOneAsync(screen);
                await _versionHub.CommitVersion(screen);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the screen with ID {ScreenId}", screen.Id);
                throw new RepositoryException(nameof(ScreenRepository), "Error creating screen", ex);
            }
        }


        public async Task<Domain.Entities.Screen> ReadScreenById(Guid id)
        {
            return await _screenCollection.Find(screen => screen.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Domain.Entities.Screen> ReadScreenByName(string name)
        {
            return await _screenCollection.Find(screen => screen.Name == name).FirstOrDefaultAsync();
        }


        public async Task<List<Domain.Entities.Screen>> GetScreensList()
        {
            try
            {
                return await _screenCollection.Find(screen => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the screen list");
                throw new RepositoryException(nameof(ScreenRepository), "Error getting screen list", ex);
            }

        }

        public async Task<List<Domain.Entities.Screen>> GetScreensListByStrainId(Guid strainId)
        {
            try
            {
                return await _screenCollection.Find(screen => screen.StrainId == strainId).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the screen list");
                throw new RepositoryException(nameof(ScreenRepository), "Error getting screen list", ex);
            }

        }



        public async Task UpdateScreen(Domain.Entities.Screen screen)
        {
            ArgumentNullException.ThrowIfNull(screen);

            try
            {
                _logger.LogInformation("UpdateScreen: Updating screen {ScreenId}, {Screen}", screen.Id, screen.ToJson());
                await _screenCollection.ReplaceOneAsync(t => t.Id == screen.Id, screen);
                await _versionHub.CommitVersion(screen);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the screen with ID {ScreenId}", screen.Id);
                throw new RepositoryException(nameof(ScreenRepository), "Error updating screen", ex);
            }
        }

        public async Task DeleteScreen(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                _logger.LogInformation("DeleteScreen: Deleting screen {ScreenId}", id);
                await _screenCollection.DeleteOneAsync(t => t.Id == id);
                await _versionHub.ArchiveEntity(id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the screen with ID {ScreenId}", id);
                throw new RepositoryException(nameof(ScreenRepository), "Error deleting screen", ex);
            }
        }


        public async Task<ScreenRevision> GetScreenRevisions(Guid Id)
        {
            var screenRevision = await _versionHub.GetVersions(Id);
            return screenRevision;
        }
    }
}