

using Daikon.Shared.VM.Screen;

namespace Daikon.Shared.APIClients.Screen
{
    public interface IScreenAPI
    {
        public Task<ScreenVM> GetById(Guid id, bool forceRefresh = false);
    }
}