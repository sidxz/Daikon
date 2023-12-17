
using Horizon.Domain.Screens;

namespace Horizon.Application.Contracts.Persistance
{
    public interface IGraphRepositoryForScreen
    {
        Task CreateIndexesAsync();
        Task AddScreenToGraph(Screen screen);
        Task UpdateScreenOfGraph(Screen screen);
        Task UpdateAssociatedTargetsOfScreen(Screen screen);
        Task DeleteScreenFromGraph(string screenId);
    }
}