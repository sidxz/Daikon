using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daikon.VersionStore.Settings
{
    public interface IVersionDatabaseSettings<VersionEntityModel>
    {
        string ConnectionString { get; }
        string DatabaseName { get; }
        string CollectionName { get; }
    }
}