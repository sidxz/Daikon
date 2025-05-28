using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daikon.EventStore.Settings
{
    public interface IEventDatabaseSettings
    {
        string ConnectionString { get; }
        string DatabaseName { get; }
    }
}