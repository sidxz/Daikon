using CQRS.Core.Domain;
using MongoDB.Bson.Serialization.Attributes;

namespace Daikon.EventStore.Event
{
    /*
     Abstract base class for all domain events in the event-sourced system.
     Inherits metadata fields from DocMetadata.
     Provides common properties like ID, version, and event type.
    */
    [BsonDiscriminator(Required = true)]
    public abstract class BaseEvent : DocMetadata
    {
        /*
         Constructor that sets the event type (used for serialization and dispatching).
        */
        protected BaseEvent(string type)
        {
            this.Type = type;
        }

        /* Unique identifier for this event instance */
        public Guid Id { get; set; }

        /* ID of the user who initiated the request */
        public Guid RequestorUserId { get; set; }

        /* Version of the event within the aggregate's event stream */
        public int Version { get; set; }

        /* Event type name (used for reflection and deserialization) */
        public string Type { get; set; }

        /*
         Serializes the event to JSON using System.Text.Json.

         Returns:
         - A string representing the JSON-serialized event.
        */
        public string ToJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
