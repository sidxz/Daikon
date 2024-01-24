
using System.Text.Json;
using System.Text.Json.Serialization;
using CQRS.Core.Event;
using CQRS.Core.Exceptions;
using Daikon.Events.Screens;

namespace Screen.Infrastructure.Query.Converters
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
                "ScreenCreatedEvent" => JsonSerializer.Deserialize<ScreenCreatedEvent>(json, options),
                "ScreenUpdatedEvent" => JsonSerializer.Deserialize<ScreenUpdatedEvent>(json, options),
                "ScreenDeletedEvent" => JsonSerializer.Deserialize<ScreenDeletedEvent>(json, options),
                "ScreenRenamedEvent" => JsonSerializer.Deserialize<ScreenRenamedEvent>(json, options),
                "ScreenAssociatedTargetsUpdatedEvent" => JsonSerializer.Deserialize<ScreenAssociatedTargetsUpdatedEvent>(json, options),
                "ScreenRunAddedEvent" => JsonSerializer.Deserialize<ScreenRunAddedEvent>(json, options),
                "ScreenRunUpdatedEvent" => JsonSerializer.Deserialize<ScreenRunUpdatedEvent>(json, options),
                "ScreenRunDeletedEvent" => JsonSerializer.Deserialize<ScreenRunDeletedEvent>(json, options),
                "HitCollectionCreatedEvent" => JsonSerializer.Deserialize<HitCollectionCreatedEvent>(json, options),
                "HitCollectionUpdatedEvent" => JsonSerializer.Deserialize<HitCollectionUpdatedEvent>(json, options),
                "HitCollectionDeletedEvent" => JsonSerializer.Deserialize<HitCollectionDeletedEvent>(json, options),
                "HitCollectionRenamedEvent" => JsonSerializer.Deserialize<HitCollectionRenamedEvent>(json, options),
                "HitCollectionAssociatedScreenUpdatedEvent" => JsonSerializer.Deserialize<HitCollectionAssociatedScreenUpdatedEvent>(json, options),
                "HitAddedEvent" => JsonSerializer.Deserialize<HitAddedEvent>(json, options),
                "HitUpdatedEvent" => JsonSerializer.Deserialize<HitUpdatedEvent>(json, options),
                "HitDeletedEvent" => JsonSerializer.Deserialize<HitDeletedEvent>(json, options),




                _ => throw new UnknownEventDiscriminatorException($"Unknown discriminator value {typeDiscriminator}"),
            };
        }

        public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}