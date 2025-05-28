
using UserPreferences.Domain.Table;

namespace UserPreferences.Application.Contracts.Persistence
{
    public interface ITableGlobalConfigRepository
    {
        Task<TableGlobalConfig?> GetByTableInstanceAsync(string tableType, Guid tableInstanceId);
        Task InsertAsync(TableGlobalConfig entity);
        Task ReplaceAsync(string tableType, Guid tableInstanceId, TableGlobalConfig updated);
        Task DeleteAsync(string tableType, Guid tableInstanceId);
    }
}