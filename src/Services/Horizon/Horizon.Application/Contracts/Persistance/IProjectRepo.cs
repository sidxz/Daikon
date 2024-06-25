


using Horizon.Domain.Projects;

namespace Horizon.Application.Contracts.Persistance
{
    public interface IProjectRepo
    {
        Task CreateIndexesAsync();
        Task CreateConstraintsAsync();
        Task Create(Project project);
        Task Update(Project project);
        Task Rename(Project project);
        Task Delete(string projectId);
        Task UpdateHitAssessmentAssociation(Project project);

        Task AddProjectCEvo(ProjectCompoundEvolution projectCompoundEvolution);
        Task UpdateProjectCEvo(ProjectCompoundEvolution projectCompoundEvolution);
        Task DeleteProjectCEvo(string compoundEvoId);
    }
}