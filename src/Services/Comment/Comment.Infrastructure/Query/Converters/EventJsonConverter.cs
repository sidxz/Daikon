
using System.Text.Json;
using System.Text.Json.Serialization;
using CQRS.Core.Event;
using CQRS.Core.Exceptions;
using Daikon.Events.Comment;

namespace Comment.Infrastructure.Query.Converters
{
    public class EventJSONConverter : JsonConverter<BaseEvent>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            // Check if typeToConvert is a subclass of BaseEvent
            // This is needed because JsonSerializer.Deserialize() will call this method
            // for every type in the inheritance hierarchy of the type you are trying to deserialize.
            return typeToConvert.IsAssignableFrom(typeof(BaseEvent));
        }
        public override BaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!JsonDocument.TryParseValue(ref reader, out var doc))
            {
                throw new JsonException($"Failed to parse JSON. {nameof(JsonDocument)}");
            }

            if (!doc.RootElement.TryGetProperty("Type", out var type))
            {
                throw new JsonException($"Could not detect Type discriminator {nameof(type)}");
            }

            var typeDiscriminator = type.GetString();
            var json = doc.RootElement.GetRawText();

            return typeDiscriminator switch
            {
                "CommentCreatedEvent" => JsonSerializer.Deserialize<CommentCreatedEvent>(json, options),
                "CommentUpdatedEvent" => JsonSerializer.Deserialize<CommentUpdatedEvent>(json, options),
                "CommentDeletedEvent" => JsonSerializer.Deserialize<CommentDeletedEvent>(json, options),
                "CommentReplyAddedEvent" => JsonSerializer.Deserialize<CommentReplyAddedEvent>(json, options),
                "CommentReplyUpdatedEvent" => JsonSerializer.Deserialize<CommentReplyUpdatedEvent>(json, options),
                "CommentReplyDeletedEvent" => JsonSerializer.Deserialize<CommentReplyDeletedEvent>(json, options),

                _ => throw new UnknownEventDiscriminatorException($"Unknown discriminator value {typeDiscriminator}"),
            };
        }

        public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}