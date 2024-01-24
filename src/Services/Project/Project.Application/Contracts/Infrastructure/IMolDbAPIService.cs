
namespace Project.Application.Contracts.Infrastructure
{
    public interface IMolDbAPIService
    {
        public Task<Guid> RegisterCompound(string name, string initialCompoundStructure);
    }
}