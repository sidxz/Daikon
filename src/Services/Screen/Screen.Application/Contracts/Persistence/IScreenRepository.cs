
namespace Screen.Application.Contracts.Persistence
{
    public interface IScreenRepository
    {
        Task CreateScreen(Domain.Entities.Screen screen);
        Task<Domain.Entities.Screen> ReadScreenByName(string name);
        Task<Domain.Entities.Screen> ReadScreenById(Guid id);

        Task<List<Domain.Entities.Screen>> GetScreensList();
        Task<List<Domain.Entities.Screen>> GetScreensListByStrainId(Guid strainId);
        Task UpdateScreen(Domain.Entities.Screen screen);
        Task DeleteScreen(Domain.Entities.Screen screen);
        Task<Domain.EntityRevisions.ScreenRevision> GetScreenRevisions(Guid Id);
    }
}