
using Daikon.Shared.VM.Project;

namespace Daikon.Shared.APIClients.Project
{
    public interface IProjectAPI
    {
        public Task<ProjectVM> GetById(Guid id, bool forceRefresh = false);
        public Task<List<ProjectListVM>> GetList(bool forceRefresh = false);
    }
}