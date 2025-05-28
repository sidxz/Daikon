
using UserPreferences.Domain.Table;

namespace UserPreferences.Application.Contracts.Persistence
{
    public interface ITableUserCustomizationRepository
    {
        Task<TableUserCustomization?> GetByUserAsync(string tableType, Guid tableInstanceId, Guid userId);
        Task InsertAsync(TableUserCustomization entity);
        Task ReplaceAsync(string tableType, Guid tableInstanceId, Guid userId, TableUserCustomization updated);
        Task DeleteAsync(string tableType, Guid tableInstanceId, Guid userId);
    }
}