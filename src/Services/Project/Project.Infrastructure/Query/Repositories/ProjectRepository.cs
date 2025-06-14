
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Project.Application.Contracts.Persistence;

namespace Project.Infrastructure.Query.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IMongoCollection<Domain.Entities.Project> _projectCollection;
        private readonly ILogger<ProjectRepository> _logger;

        public ProjectRepository(IConfiguration configuration, ILogger<ProjectRepository> logger )
        {
            var client = new MongoClient(configuration.GetValue<string>("ProjectMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("ProjectMongoDbSettings:DatabaseName"));
            _projectCollection = database.GetCollection<Domain.Entities.Project>(configuration.GetValue<string>("ProjectMongoDbSettings:ProjectCollectionName"));
            _projectCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Project>(Builders<Domain.Entities.Project>.IndexKeys.Ascending(t => t.Name), new CreateIndexOptions { Unique = false }));
            _projectCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Project>(Builders<Domain.Entities.Project>.IndexKeys.Ascending(t => t.StrainId), new CreateIndexOptions { Unique = false }));
            _projectCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Project>(Builders<Domain.Entities.Project>.IndexKeys.Descending(t => t.DateCreated), new CreateIndexOptions { Unique = false }));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        public async Task CreateProject(Domain.Entities.Project project)
        {

            ArgumentNullException.ThrowIfNull(project);

            try
            {
                _logger.LogInformation("CreateProject: Creating project {ProjectId}, {Project}", project.Id, project.ToJson());
                await _projectCollection.InsertOneAsync(project);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the project with ID {ProjectId}", project.Id);
                throw new RepositoryException(nameof(ProjectRepository), "Error creating project", ex);
            }
        }


        public async Task<Domain.Entities.Project> ReadProjectById(Guid id)
        {
            return await _projectCollection.Find(project => project.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Domain.Entities.Project> ReadProjectByName(string name)
        {
            return await _projectCollection.Find(project => project.Name == name).FirstOrDefaultAsync();
        }


        public async Task<List<Domain.Entities.Project>> GetProjectList()
        {
            try
            {
                return await _projectCollection.Find(project => true)
                .SortBy(project => project.Name)
                .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the project list");
                throw new RepositoryException(nameof(ProjectRepository), "Error getting project list", ex);
            }

        }

        public async Task<List<Domain.Entities.Project>> GetProjectListByStrainId(Guid strainId)
        {
            try
            {
                return await _projectCollection.Find(project => project.StrainId == strainId)
                .SortBy(project => project.Name)
                .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the project list");
                throw new RepositoryException(nameof(ProjectRepository), "Error getting project list", ex);
            }

        }



        public async Task UpdateProject(Domain.Entities.Project project)
        {
            ArgumentNullException.ThrowIfNull(project);

            try
            {
                _logger.LogInformation("UpdateProject: Updating project {ProjectId}, {Project}", project.Id, project.ToJson());
                await _projectCollection.ReplaceOneAsync(t => t.Id == project.Id, project);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the project with ID {ProjectId}", project.Id);
                throw new RepositoryException(nameof(ProjectRepository), "Error updating project", ex);
            }
        }

        public async Task DeleteProject(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                _logger.LogInformation("DeleteProject: Deleting project {ProjectId}", id);
                await _projectCollection.DeleteOneAsync(t => t.Id == id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the project with ID {ProjectId}", id);
                throw new RepositoryException(nameof(ProjectRepository), "Error deleting project", ex);
            }
        }
    }
}