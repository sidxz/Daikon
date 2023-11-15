using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

/// <summary>
/// Represents an event model that contains information about an event.
/// </summary>
namespace CQRS.Core.Event
{
    public class EventModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid AggregateIdentifier { get; set; }
        public required string AggregateType { get; set; }
        public int Version { get; set; }
        public required string EventType { get; set; }
        public required BaseEvent EventData { get; set; }

        public string? UserId { get; set; } // User who triggered the event
        public string? SessionId { get; set; } // Session in which the event was triggered
        public string? Source { get; set; } // Source of the event (service/component name)
        public Guid CorrelationId { get; set; } // Links all events from a single transaction
        public Guid CausationId { get; set; } // The event that caused this event
        public string? Metadata { get; set; } // Additional metadata in JSON/XML format
        public string? TenantId { get; set; } // In multi-tenant architecture
        public string? EventState { get; set; } // State of the event processing
    
    }
}