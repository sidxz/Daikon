
namespace Screen.Application.Contracts.Persistence
{
    public interface IScreenRunRepository
    {
        Task CreateScreenRun(Domain.Entities.ScreenRun screenRun);

        Task<Domain.Entities.ScreenRun> ReadScreenRunById(Guid id);
        Task<List<Domain.Entities.ScreenRun>> GetScreenRunList();
        Task<List<Domain.Entities.ScreenRun>> GetScreenRunsListByScreenId(Guid screenId);
        Task UpdateScreenRun(Domain.Entities.ScreenRun screenRun);
        Task DeleteScreenRun(Guid id);
        Task DeleteScreenRunsByScreenId(Guid screenId);
        Task<Domain.EntityRevisions.ScreenRunRevision> GetScreenRunRevisions(Guid Id);
       
    }
}