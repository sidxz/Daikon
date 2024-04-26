
using System.Text.Json;
using System.Text.Json.Serialization;
using CQRS.Core.Event;
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Daikon.Events.HitAssessment;
using Daikon.Events.MLogix;
using Daikon.Events.Project;
using Daikon.Events.Screens;
using Daikon.Events.Strains;
using Daikon.Events.Targets;

namespace Horizon.Infrastructure.Query.Converters
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

                "TargetCreatedEvent" => JsonSerializer.Deserialize<TargetCreatedEvent>(json, options),
                "TargetUpdatedEvent" => JsonSerializer.Deserialize<TargetUpdatedEvent>(json, options),
                "TargetDeletedEvent" => JsonSerializer.Deserialize<TargetDeletedEvent>(json, options),
                "TargetRenamedEvent" => JsonSerializer.Deserialize<TargetRenamedEvent>(json, options),
                "TargetAssociatedGenesUpdatedEvent" => JsonSerializer.Deserialize<TargetAssociatedGenesUpdatedEvent>(json, options),
                
                "ScreenCreatedEvent" => JsonSerializer.Deserialize<ScreenCreatedEvent>(json, options),
                "ScreenUpdatedEvent" => JsonSerializer.Deserialize<ScreenUpdatedEvent>(json, options),
                "ScreenDeletedEvent" => JsonSerializer.Deserialize<ScreenDeletedEvent>(json, options),
                "ScreenRenamedEvent" => JsonSerializer.Deserialize<ScreenRenamedEvent>(json, options),
                "ScreenAssociatedTargetsUpdatedEvent" => JsonSerializer.Deserialize<ScreenAssociatedTargetsUpdatedEvent>(json, options),
                "HitCollectionCreatedEvent" => JsonSerializer.Deserialize<HitCollectionCreatedEvent>(json, options),
                "HitCollectionUpdatedEvent" => JsonSerializer.Deserialize<HitCollectionUpdatedEvent>(json, options),
                "HitCollectionDeletedEvent" => JsonSerializer.Deserialize<HitCollectionDeletedEvent>(json, options),
                "HitCollectionRenamedEvent" => JsonSerializer.Deserialize<HitCollectionRenamedEvent>(json, options),
                "HitCollectionAssociatedScreenUpdatedEvent" => JsonSerializer.Deserialize<HitCollectionAssociatedScreenUpdatedEvent>(json, options),
                "HitAddedEvent" => JsonSerializer.Deserialize<HitAddedEvent>(json, options),
                "HitUpdatedEvent" => JsonSerializer.Deserialize<HitUpdatedEvent>(json, options),
                "HitDeletedEvent" => JsonSerializer.Deserialize<HitDeletedEvent>(json, options),
                "MoleculeCreatedEvent" => JsonSerializer.Deserialize<MoleculeCreatedEvent>(json, options),

                "HaCreatedEvent" => JsonSerializer.Deserialize<HaCreatedEvent>(json, options),
                "HaUpdatedEvent" => JsonSerializer.Deserialize<HaUpdatedEvent>(json, options),
                "HaDeletedEvent" => JsonSerializer.Deserialize<HaDeletedEvent>(json, options),
                "HaCompoundEvolutionAddedEvent" => JsonSerializer.Deserialize<HaCompoundEvolutionAddedEvent>(json, options),
                "HaCompoundEvolutionUpdatedEvent" => JsonSerializer.Deserialize<HaCompoundEvolutionUpdatedEvent>(json, options),
                "HaCompoundEvolutionDeletedEvent" => JsonSerializer.Deserialize<HaCompoundEvolutionDeletedEvent>(json, options),

                "ProjectCreatedEvent" => JsonSerializer.Deserialize<ProjectCreatedEvent>(json, options),
                "ProjectUpdatedEvent" => JsonSerializer.Deserialize<ProjectUpdatedEvent>(json, options),
                "ProjectDeletedEvent" => JsonSerializer.Deserialize<ProjectDeletedEvent>(json, options),
                "ProjectAssociationUpdatedEvent" => JsonSerializer.Deserialize<ProjectAssociationUpdatedEvent>(json, options),
                
                
                
                _ => throw new UnknownEventDiscriminatorException($"Unknown discriminator value {typeDiscriminator}"),
            };
        }

        public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}