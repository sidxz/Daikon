
using Horizon.Domain.Screens;

namespace Horizon.Application.Contracts.Persistence
{
    public interface IGraphRepositoryForScreen
    {
        Task CreateIndexesAsync();
        Task CreateConstraintsAsync();
        Task AddScreen(Screen screen);
        Task UpdateScreen(Screen screen);
        Task UpdateAssociatedTargetsOfScreen(Screen screen);
        Task DeleteScreen(string screenId);
        Task RenameScreen(string screenId, string newName);
    }
}