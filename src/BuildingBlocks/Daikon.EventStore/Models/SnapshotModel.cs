using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Daikon.EventStore.Models
{
    public class SnapshotModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public Guid AggregateIdentifier { get; set; }

        [BsonRequired]
        public string AggregateType { get; set; }

        [BsonRequired]
        public string Data { get; set; } // Serialized aggregate state

        [BsonRequired]
        public int Version { get; set; }

        [BsonRequired]
        public DateTime TimeStamp { get; set; }

        public string? TenantId { get; set; }
        public string? Metadata { get; set; }
    }
}
