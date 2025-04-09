
using CQRS.Core.Domain;
using MongoDB.Bson.Serialization.Attributes;

namespace Daikon.EventStore.Event
{
    [BsonDiscriminator(Required = true)]

    public abstract class BaseEvent : DocMetadata
    {
        protected BaseEvent(string type)
        {
            this.Type = type;
        }

        public Guid Id { get; set; }
        public Guid RequestorUserId { get; set; }
        public int Version { get; set; }
        public string Type { get; set; }

        public string ToJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }

    }
}