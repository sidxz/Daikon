using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Daikon.EventStore.Models
{
    /*
     Represents a snapshot of an aggregate's state at a specific version.
     Used to speed up aggregate rehydration by avoiding full event replay.
    */
    public class SnapshotModel
    {
        /* MongoDB document ID */
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /* ID of the aggregate this snapshot belongs to */
        [BsonRequired]
        public Guid AggregateIdentifier { get; set; }

        /* Type of the aggregate */
        [BsonRequired]
        public string AggregateType { get; set; }

        /* Serialized representation of the aggregate's state */
        [BsonRequired]
        public string Data { get; set; }

        /* Version of the aggregate at the time of snapshot */
        [BsonRequired]
        public int Version { get; set; }

        /* Timestamp when the snapshot was created */
        [BsonRequired]
        public DateTime TimeStamp { get; set; }

        /* Optional: Tenant identifier in multi-tenant systems */
        public string? TenantId { get; set; }

        /* Optional: Extra metadata (e.g., trace info, source system) */
        public string? Metadata { get; set; }
    }
}
