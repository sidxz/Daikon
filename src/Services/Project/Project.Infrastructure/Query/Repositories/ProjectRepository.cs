
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Project.Application.Contracts.Persistence;
using Project.Domain.EntityRevisions;

namespace Project.Infrastructure.Query.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IMongoCollection<Domain.Entities.Project> _projectCollection;
        private readonly ILogger<ProjectRepository> _logger;
        private readonly IVersionHub<ProjectRevision> _versionHub;

        public ProjectRepository(IConfiguration configuration, ILogger<ProjectRepository> logger, IVersionHub<ProjectRevision> versionMaintainer)
        {
            var client = new MongoClient(configuration.GetValue<string>("ProjectMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("ProjectMongoDbSettings:DatabaseName"));
            _projectCollection = database.GetCollection<Domain.Entities.Project>(configuration.GetValue<string>("ProjectMongoDbSettings:ProjectCollectionName"));
            _projectCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Project>(Builders<Domain.Entities.Project>.IndexKeys.Ascending(t => t.Name), new CreateIndexOptions { Unique = false }));
            _projectCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Project>(Builders<Domain.Entities.Project>.IndexKeys.Ascending(t => t.StrainId), new CreateIndexOptions { Unique = false }));

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        public async Task CreateProject(Domain.Entities.Project screen)
        {

            ArgumentNullException.ThrowIfNull(screen);

            try
            {
                _logger.LogInformation("CreateProject: Creating screen {ProjectId}, {Project}", screen.Id, screen.ToJson());
                await _projectCollection.InsertOneAsync(screen);
                await _versionHub.CommitVersion(screen);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the screen with ID {ProjectId}", screen.Id);
                throw new RepositoryException(nameof(ProjectRepository), "Error creating screen", ex);
            }
        }


        public async Task<Domain.Entities.Project> ReadProjectById(Guid id)
        {
            return await _projectCollection.Find(screen => screen.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Domain.Entities.Project> ReadProjectByName(string name)
        {
            return await _projectCollection.Find(screen => screen.Name == name).FirstOrDefaultAsync();
        }


        public async Task<List<Domain.Entities.Project>> GetProjectList()
        {
            try
            {
                return await _projectCollection.Find(screen => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the screen list");
                throw new RepositoryException(nameof(ProjectRepository), "Error getting screen list", ex);
            }

        }

        public async Task<List<Domain.Entities.Project>> GetProjectListByStrainId(Guid strainId)
        {
            try
            {
                return await _projectCollection.Find(screen => screen.StrainId == strainId).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the screen list");
                throw new RepositoryException(nameof(ProjectRepository), "Error getting screen list", ex);
            }

        }



        public async Task UpdateProject(Domain.Entities.Project screen)
        {
            ArgumentNullException.ThrowIfNull(screen);

            try
            {
                _logger.LogInformation("UpdateProject: Updating screen {ProjectId}, {Project}", screen.Id, screen.ToJson());
                await _projectCollection.ReplaceOneAsync(t => t.Id == screen.Id, screen);
                await _versionHub.CommitVersion(screen);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the screen with ID {ProjectId}", screen.Id);
                throw new RepositoryException(nameof(ProjectRepository), "Error updating screen", ex);
            }
        }

        public async Task DeleteProject(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                _logger.LogInformation("DeleteProject: Deleting screen {ProjectId}", id);
                await _projectCollection.DeleteOneAsync(t => t.Id == id);
                await _versionHub.ArchiveEntity(id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the screen with ID {ProjectId}", id);
                throw new RepositoryException(nameof(ProjectRepository), "Error deleting screen", ex);
            }
        }


        public async Task<ProjectRevision> GetProjectRevisions(Guid Id)
        {
            var screenRevision = await _versionHub.GetVersions(Id);
            return screenRevision;
        }
    }
}