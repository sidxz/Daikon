
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UserPreferences.Application.Contracts.Persistence;
using UserPreferences.Domain.Table;

namespace UserPreferences.Infrastructure.Repositories
{
    public class TableUserCustomizationRepository : MongoRepositoryBase<TableUserCustomization>, ITableUserCustomizationRepository
    {
        public TableUserCustomizationRepository(IConfiguration config, ILogger<TableUserCustomizationRepository> logger)
            : base(config, "TableUserCustomizations", logger) { }

        public Task<TableUserCustomization?> GetByUserAsync(string tableType, Guid tableInstanceId, Guid userId)
        {
            return FindOneAsync(x => x.TableType == tableType && x.TableInstanceId == tableInstanceId && x.UserId == userId);
        }

        public Task ReplaceAsync(string tableType, Guid tableInstanceId, Guid userId, TableUserCustomization updated)
        {
            return ReplaceAsync(x => x.TableType == tableType && x.TableInstanceId == tableInstanceId && x.UserId == userId, updated);
        }

        public Task DeleteAsync(string tableType, Guid tableInstanceId, Guid userId)
        {
            return DeleteAsync(x => x.TableType == tableType && x.TableInstanceId == tableInstanceId && x.UserId == userId);
        }
    }
}