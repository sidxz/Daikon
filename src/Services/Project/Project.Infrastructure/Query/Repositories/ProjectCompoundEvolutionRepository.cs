
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Project.Application.Contracts.Persistence;
using Project.Domain.Entities;
using Project.Domain.EntityRevisions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Project.Infrastructure.Query.Repositories
{
    public class ProjectCompoundEvolutionRepository : IProjectCompoundEvolutionRepository
    {

        private readonly IMongoCollection<ProjectCompoundEvolution> _projectCompoundEvoCollection;
        private readonly ILogger<ProjectCompoundEvolutionRepository> _logger;
        private readonly IVersionHub<ProjectCompoundEvolutionRevision> _versionHub;

        public ProjectCompoundEvolutionRepository(IConfiguration configuration, ILogger<ProjectCompoundEvolutionRepository> logger, IVersionHub<ProjectCompoundEvolutionRevision> versionMaintainer)
        {
            var client = new MongoClient(configuration.GetValue<string>("ProjectMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("ProjectMongoDbSettings:DatabaseName"));
            _projectCompoundEvoCollection = database.GetCollection<ProjectCompoundEvolution>(configuration.GetValue<string>("ProjectMongoDbSettings:ProjectCompoundEvolutionCollectionName"));
            _projectCompoundEvoCollection.Indexes.CreateOne(new CreateIndexModel<ProjectCompoundEvolution>(Builders<ProjectCompoundEvolution>.IndexKeys.Ascending(t => t.ProjectId), new CreateIndexOptions { Unique = false }));

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateProjectCompoundEvolution(ProjectCompoundEvolution projectCompoundEvolution)
        {
            ArgumentNullException.ThrowIfNull(projectCompoundEvolution);

            try
            {
                _logger.LogInformation("CreateProjectCompoundEvolution: Creating projectCompoundEvolution {ProjectCompoundEvolutionId}, {ProjectCompoundEvolution}", projectCompoundEvolution.Id, projectCompoundEvolution.ToJson());
                await _projectCompoundEvoCollection.InsertOneAsync(projectCompoundEvolution);
                await _versionHub.CommitVersion(projectCompoundEvolution);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the projectCompoundEvolution with ID {ProjectCompoundEvolutionId}", projectCompoundEvolution.Id);
                throw new RepositoryException(nameof(ProjectCompoundEvolutionRepository), "Error creating projectCompoundEvolution", ex);
            }
        }

        public async Task DeleteProjectCompoundEvolution(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                _logger.LogInformation("DeleteProjectCompoundEvolution: Deleting projectCompoundEvolution {ProjectCompoundEvolutionId}", id);
                await _projectCompoundEvoCollection.DeleteOneAsync(projectCompoundEvolution => projectCompoundEvolution.Id == id);
                await _versionHub.ArchiveEntity(id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the projectCompoundEvolution with ID {ProjectCompoundEvolutionId}", id);
                throw new RepositoryException(nameof(ProjectCompoundEvolutionRepository), "Error deleting projectCompoundEvolution", ex);
            }
        }

        public async Task<List<ProjectCompoundEvolution>> GetProjectCompoundEvolutionOfProject(Guid ProjectId)
        {
            ArgumentNullException.ThrowIfNull(ProjectId);
            try
            {
                _logger.LogInformation("GetProjectCompoundEvolutionOfProject: Getting projectCompoundEvolution of project {ProjectId}", ProjectId);
                return await _projectCompoundEvoCollection.Find(projectCompoundEvolution => projectCompoundEvolution.ProjectId == ProjectId).SortByDescending(projectCompoundEvolution => projectCompoundEvolution.EvolutionDate.Value)
                .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the projectCompoundEvolution of project with ID {ProjectId}", ProjectId);
                throw new RepositoryException(nameof(ProjectCompoundEvolutionRepository), "Error getting projectCompoundEvolution of project", ex);
            }

        }



        public async Task<ProjectCompoundEvolution> ReadProjectCompoundEvolutionById(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                _logger.LogInformation("ReadProjectCompoundEvolutionById: Reading projectCompoundEvolution {ProjectCompoundEvolutionId}", id);
                return await _projectCompoundEvoCollection.Find(projectCompoundEvolution => projectCompoundEvolution.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while reading the projectCompoundEvolution with ID {ProjectCompoundEvolutionId}", id);
                throw new RepositoryException(nameof(ProjectCompoundEvolutionRepository), "Error reading projectCompoundEvolution", ex);
            }
        }

        public async Task UpdateProjectCompoundEvolution(ProjectCompoundEvolution projectCompoundEvolution)
        {
            ArgumentNullException.ThrowIfNull(projectCompoundEvolution);
            try
            {
                _logger.LogInformation("UpdateProjectCompoundEvolution: Updating projectCompoundEvolution {ProjectCompoundEvolutionId}, {ProjectCompoundEvolution}", projectCompoundEvolution.Id, projectCompoundEvolution.ToJson());
                await _projectCompoundEvoCollection.ReplaceOneAsync(t => t.Id == projectCompoundEvolution.Id, projectCompoundEvolution);
                await _versionHub.CommitVersion(projectCompoundEvolution);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the projectCompoundEvolution with ID {ProjectCompoundEvolutionId}", projectCompoundEvolution.Id);
                throw new RepositoryException(nameof(ProjectCompoundEvolutionRepository), "Error updating projectCompoundEvolution", ex);
            }
        }


        public async Task<ProjectCompoundEvolutionRevision> GetProjectCompoundEvolutionRevisions(Guid Id)
        {
            var projectCompoundEvolutionRevision = await _versionHub.GetVersions(Id);
            return projectCompoundEvolutionRevision;
        }
    }
}