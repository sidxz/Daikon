
using Project.Domain.Entities;

namespace Project.Application.Contracts.Persistence
{
    public interface IProjectCompoundEvolutionRepository
    {
        Task CreateProjectCompoundEvolution(ProjectCompoundEvolution projectCompoundEvolution);
        Task<ProjectCompoundEvolution> ReadProjectCompoundEvolutionById(Guid id);
        Task<List<ProjectCompoundEvolution>> GetProjectCompoundEvolutionOfProject(Guid ProjectId);
        public Task<Dictionary<Guid, List<ProjectCompoundEvolution>>> GetProjectCompoundEvolutionsOfProjects(List<Guid> projectIDs);
        Task UpdateProjectCompoundEvolution(ProjectCompoundEvolution projectCompoundEvolution);
        Task DeleteProjectCompoundEvolution(Guid id);
    }
}