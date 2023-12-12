using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CQRS.Core.Event;
using Daikon.Events.Gene;
using Daikon.Events.Strains;

namespace Gene.Infrastructure.Query.Converters
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
                "GeneCreatedEvent" => JsonSerializer.Deserialize<GeneCreatedEvent>(json, options),
                "GeneUpdatedEvent" => JsonSerializer.Deserialize<GeneUpdatedEvent>(json, options),
                "GeneDeletedEvent" => JsonSerializer.Deserialize<GeneDeletedEvent>(json, options),

                "StrainCreatedEvent" => JsonSerializer.Deserialize<StrainCreatedEvent>(json, options),
                "StrainUpdatedEvent" => JsonSerializer.Deserialize<StrainUpdatedEvent>(json, options),
                "StrainDeletedEvent" => JsonSerializer.Deserialize<StrainDeletedEvent>(json, options),
                
                "GeneEssentialityAddedEvent" => JsonSerializer.Deserialize<GeneEssentialityAddedEvent>(json, options),
                "GeneEssentialityUpdatedEvent" => JsonSerializer.Deserialize<GeneEssentialityUpdatedEvent>(json, options),
                "GeneEssentialityDeletedEvent" => JsonSerializer.Deserialize<GeneEssentialityDeletedEvent>(json, options),

                "GeneProteinProductionAddedEvent" => JsonSerializer.Deserialize<GeneProteinProductionAddedEvent>(json, options),
                "GeneProteinProductionUpdatedEvent" => JsonSerializer.Deserialize<GeneProteinProductionUpdatedEvent>(json, options),
                "GeneProteinProductionDeletedEvent" => JsonSerializer.Deserialize<GeneProteinProductionDeletedEvent>(json, options),

                "GeneProteinActivityAssayAddedEvent" => JsonSerializer.Deserialize<GeneProteinActivityAssayAddedEvent>(json, options),
                "GeneProteinActivityAssayUpdatedEvent" => JsonSerializer.Deserialize<GeneProteinActivityAssayUpdatedEvent>(json, options),
                "GeneProteinActivityAssayDeletedEvent" => JsonSerializer.Deserialize<GeneProteinActivityAssayDeletedEvent>(json, options),
                
                _ => throw new JsonException($"Unknown discriminator value {typeDiscriminator}"),
            };
        }

        public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}