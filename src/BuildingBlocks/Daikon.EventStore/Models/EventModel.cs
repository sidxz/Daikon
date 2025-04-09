using Daikon.EventStore.Event;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Daikon.EventStore.Models
{
    /*
     Represents a persisted domain event in the event store.
     Contains metadata for traceability, auditing, and versioning.
    */
    public class EventModel
    {
        /* MongoDB document ID */
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /* Timestamp when the event was persisted */
        public DateTime TimeStamp { get; set; }

        /* ID of the aggregate the event belongs to */
        public Guid AggregateIdentifier { get; set; }

        /* Name of the aggregate type */
        public required string AggregateType { get; set; }

        /* Version of the event in the aggregate's event stream */
        public int Version { get; set; }

        /* Type name of the domain event */
        public required string EventType { get; set; }

        /* Serialized event payload (actual domain event data) */
        public required BaseEvent EventData { get; set; }

        /* Optional: ID of the user who triggered the event */
        public string? UserId { get; set; }

        /* Optional: Session ID where the event originated */
        public string? SessionId { get; set; }

        /* Optional: Service or component that produced the event */
        public string? Source { get; set; }

        /* Optional: Correlation ID for tracing across services */
        public Guid CorrelationId { get; set; }

        /* Optional: ID of the event that caused this one */
        public Guid CausationId { get; set; }

        /* Optional: Additional metadata (e.g., JSON) */
        public string? Metadata { get; set; }

        /* Optional: Tenant identifier for multi-tenant systems */
        public string? TenantId { get; set; }

        /* Optional: Processing state (e.g., pending, processed) */
        public string? EventState { get; set; }
    }
}
