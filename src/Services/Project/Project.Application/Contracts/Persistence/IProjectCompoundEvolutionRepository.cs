
namespace Project.Application.Contracts.Persistence
{
    public interface IProjectCompoundEvolutionRepository
    {
        Task CreateProjectCompoundEvolution(Domain.Entities.ProjectCompoundEvolution projectCompoundEvolution);
        Task<Domain.Entities.ProjectCompoundEvolution> ReadProjectCompoundEvolutionById(Guid id);
        Task<List<Domain.Entities.ProjectCompoundEvolution>> GetProjectCompoundEvolutionOfProject(Guid ProjectId);
        Task UpdateProjectCompoundEvolution(Domain.Entities.ProjectCompoundEvolution projectCompoundEvolution);
        Task DeleteProjectCompoundEvolution(Guid id);
        Task<Domain.EntityRevisions.ProjectCompoundEvolutionRevision> GetProjectCompoundEvolutionRevisions(Guid Id);
    }
}