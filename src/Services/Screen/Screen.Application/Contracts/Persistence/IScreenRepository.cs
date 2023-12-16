
namespace Screen.Application.Contracts.Persistence
{
    public interface IScreenRepository
    {
        Task<Domain.Entities.Screen> ReadScreenByName(string name);
        Task<Domain.Entities.Screen> ReadScreenById(Guid id);
    }
}