
namespace Screen.Application.Contracts.Persistence
{
    public interface IScreenRunRepository
    {
        Task CreateScreenRun(Domain.Entities.ScreenRun screenRun);

        Task<Domain.Entities.ScreenRun> ReadScreenRunById(Guid id);
        Task<List<Domain.Entities.ScreenRun>> GetScreensRunList();
        Task<List<Domain.Entities.ScreenRun>> GetScreensRunListByScreenId(Guid screenId);
        Task UpdateScreenRun(Domain.Entities.ScreenRun screenRun);
        Task DeleteScreenRun(Guid id);
        Task<Domain.EntityRevisions.ScreenRunRevision> GetScreenRunRevisions(Guid Id);
       
    }
}