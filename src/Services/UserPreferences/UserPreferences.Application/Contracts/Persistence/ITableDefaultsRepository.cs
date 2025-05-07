using UserPreferences.Domain.Table;

namespace UserPreferences.Application.Contracts.Persistence
{
    public interface ITableDefaultsRepository
    {
        Task<TableDefaults> GetByTableTypeAsync(string tableType);
        Task<List<TableDefaults>> GetAllAsync();
        Task InsertAsync(TableDefaults entity);
        Task ReplaceAsync(string tableType, TableDefaults updated);
        Task DeleteAsync(string tableType);
    }
}
