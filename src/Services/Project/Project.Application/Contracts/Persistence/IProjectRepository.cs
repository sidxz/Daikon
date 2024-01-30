
namespace Project.Application.Contracts.Persistence
{
    public interface IProjectRepository
    {
        Task CreateProject(Domain.Entities.Project project);
        Task<Domain.Entities.Project> ReadProjectByName(string name);
        Task<Domain.Entities.Project> ReadProjectById(Guid id);

        Task<List<Domain.Entities.Project>> GetProjectList();
        Task<List<Domain.Entities.Project>> GetProjectListByStrainId(Guid strainId);
        Task UpdateProject(Domain.Entities.Project project);
        Task DeleteProject(Guid id);
        Task<Domain.EntityRevisions.ProjectRevision> GetProjectRevisions(Guid Id);
    }
}