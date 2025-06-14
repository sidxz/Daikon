using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Daikon.EventStore.Event;
using CQRS.Core.Exceptions;
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

                "GeneHypomorphAddedEvent" => JsonSerializer.Deserialize<GeneHypomorphAddedEvent>(json, options),
                "GeneHypomorphUpdatedEvent" => JsonSerializer.Deserialize<GeneHypomorphUpdatedEvent>(json, options),
                "GeneHypomorphDeletedEvent" => JsonSerializer.Deserialize<GeneHypomorphDeletedEvent>(json, options),

                "GeneCrispriStrainAddedEvent" => JsonSerializer.Deserialize<GeneCrispriStrainAddedEvent>(json, options),
                "GeneCrispriStrainUpdatedEvent" => JsonSerializer.Deserialize<GeneCrispriStrainUpdatedEvent>(json, options),
                "GeneCrispriStrainDeletedEvent" => JsonSerializer.Deserialize<GeneCrispriStrainDeletedEvent>(json, options),

                "GeneResistanceMutationAddedEvent" => JsonSerializer.Deserialize<GeneResistanceMutationAddedEvent>(json, options),
                "GeneResistanceMutationUpdatedEvent" => JsonSerializer.Deserialize<GeneResistanceMutationUpdatedEvent>(json, options),
                "GeneResistanceMutationDeletedEvent" => JsonSerializer.Deserialize<GeneResistanceMutationDeletedEvent>(json, options),

                "GeneVulnerabilityAddedEvent" => JsonSerializer.Deserialize<GeneVulnerabilityAddedEvent>(json, options),
                "GeneVulnerabilityUpdatedEvent" => JsonSerializer.Deserialize<GeneVulnerabilityUpdatedEvent>(json, options),
                "GeneVulnerabilityDeletedEvent" => JsonSerializer.Deserialize<GeneVulnerabilityDeletedEvent>(json, options),

                "GeneUnpublishedStructuralInformationAddedEvent" => JsonSerializer.Deserialize<GeneUnpublishedStructuralInformationAddedEvent>(json, options),
                "GeneUnpublishedStructuralInformationUpdatedEvent" => JsonSerializer.Deserialize<GeneUnpublishedStructuralInformationUpdatedEvent>(json, options),
                "GeneUnpublishedStructuralInformationDeletedEvent" => JsonSerializer.Deserialize<GeneUnpublishedStructuralInformationDeletedEvent>(json, options),

                "GeneExpansionPropAddedEvent" => JsonSerializer.Deserialize<GeneExpansionPropAddedEvent>(json, options),
                "GeneExpansionPropUpdatedEvent" => JsonSerializer.Deserialize<GeneExpansionPropUpdatedEvent>(json, options),
                "GeneExpansionPropDeletedEvent" => JsonSerializer.Deserialize<GeneExpansionPropDeletedEvent>(json, options),

                _ => throw new UnknownEventDiscriminatorException($"Unknown discriminator value {typeDiscriminator}"),
            };
        }

        public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}