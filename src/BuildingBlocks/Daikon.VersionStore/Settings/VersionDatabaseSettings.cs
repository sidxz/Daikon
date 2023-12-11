using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daikon.VersionStore.Settings
{
    public class VersionDatabaseSettings <VersionEntityModel> : IVersionDatabaseSettings<VersionEntityModel>
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
        public required string CollectionName { get; set; }

    }
}