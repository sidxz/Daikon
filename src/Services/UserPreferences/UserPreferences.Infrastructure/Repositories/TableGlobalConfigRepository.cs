
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UserPreferences.Application.Contracts.Persistence;
using UserPreferences.Domain.Table;

namespace UserPreferences.Infrastructure.Repositories
{
    public class TableGlobalConfigRepository : MongoRepositoryBase<TableGlobalConfig>, ITableGlobalConfigRepository
    {
        public TableGlobalConfigRepository(IConfiguration config, ILogger<TableGlobalConfigRepository> logger)
            : base(config, "TableGlobalConfigs", logger) { }

        public Task<TableGlobalConfig?> GetByTableInstanceAsync(string tableType, Guid tableInstanceId)
        {
            return FindOneAsync(x => x.TableType == tableType && x.TableInstanceId == tableInstanceId);
        }

        public Task ReplaceAsync(string tableType, Guid tableInstanceId, TableGlobalConfig updated)
        {
            return ReplaceAsync(x => x.TableType == tableType && x.TableInstanceId == tableInstanceId, updated);
        }

        public Task DeleteAsync(string tableType, Guid tableInstanceId)
        {
            return DeleteAsync(x => x.TableType == tableType && x.TableInstanceId == tableInstanceId);
        }
    }
}