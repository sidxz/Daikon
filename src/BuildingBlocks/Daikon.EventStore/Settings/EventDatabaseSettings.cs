using Daikon.EventStore.Settings;

namespace Daikon.EventStore.Settings
{
    public class EventDatabaseSettings : IEventDatabaseSettings
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
        public required string CollectionName { get; set; }
    }
}