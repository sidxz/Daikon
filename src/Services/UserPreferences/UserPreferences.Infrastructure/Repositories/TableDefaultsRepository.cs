
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UserPreferences.Application.Contracts.Persistence;
using UserPreferences.Domain.Table;

namespace UserPreferences.Infrastructure.Repositories
{
    public class TableDefaultsRepository : MongoRepositoryBase<TableDefaults>, ITableDefaultsRepository
    {
        public TableDefaultsRepository(IConfiguration config, ILogger<TableDefaultsRepository> logger)
            : base(config, "TableDefaults", logger) { }

        public Task<TableDefaults?> GetByTableTypeAsync(string tableType)
        {
            return FindOneAsync(x => x.TableType == tableType);
        }
        public Task<List<TableDefaults>> GetAllAsync()
        {
            return FindAllAsync();
        }

        public Task ReplaceAsync(string tableType, TableDefaults updated)
        {
            return ReplaceAsync(x => x.TableType == tableType, updated);
        }

        public Task DeleteAsync(string tableType)
        {
            return DeleteAsync(x => x.TableType == tableType);
        }
    }
}