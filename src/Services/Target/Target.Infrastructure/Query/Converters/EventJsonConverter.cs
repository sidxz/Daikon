
using System.Text.Json;
using System.Text.Json.Serialization;
using Daikon.EventStore.Event;
using CQRS.Core.Exceptions;
using Daikon.Events.Targets;

namespace Target.Infrastructure.Query.Converters
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
                "TargetCreatedEvent" => JsonSerializer.Deserialize<TargetCreatedEvent>(json, options),
                "TargetUpdatedEvent" => JsonSerializer.Deserialize<TargetUpdatedEvent>(json, options),
                "TargetDeletedEvent" => JsonSerializer.Deserialize<TargetDeletedEvent>(json, options),
                "TargetRenamedEvent" => JsonSerializer.Deserialize<TargetRenamedEvent>(json, options),
                "TargetAssociatedGenesUpdatedEvent" => JsonSerializer.Deserialize<TargetAssociatedGenesUpdatedEvent>(json, options),
                
                "TargetPromotionQuestionnaireSubmittedEvent" => JsonSerializer.Deserialize<TargetPromotionQuestionnaireSubmittedEvent>(json, options),
                "TargetPromotionQuestionnaireUpdatedEvent" => JsonSerializer.Deserialize<TargetPromotionQuestionnaireUpdatedEvent>(json, options),
                "TargetPromotionQuestionnaireDeletedEvent" => JsonSerializer.Deserialize<TargetPromotionQuestionnaireDeletedEvent>(json, options),

                "TargetToxicologyAddedEvent" => JsonSerializer.Deserialize<TargetToxicologyAddedEvent>(json, options),
                "TargetToxicologyUpdatedEvent" => JsonSerializer.Deserialize<TargetToxicologyUpdatedEvent>(json, options),
                "TargetToxicologyDeletedEvent" => JsonSerializer.Deserialize<TargetToxicologyDeletedEvent>(json, options),
                

                
                _ => throw new UnknownEventDiscriminatorException($"Unknown discriminator value {typeDiscriminator}"),
            };
        }

        public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}