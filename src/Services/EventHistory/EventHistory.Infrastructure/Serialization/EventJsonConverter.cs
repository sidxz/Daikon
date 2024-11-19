// using System;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using CQRS.Core.Event;
// using CQRS.Core.Exceptions;
// using Daikon.Events.Comment;
// using Daikon.Events.Gene;
// using Daikon.Events.HitAssessment;
// using Daikon.Events.MLogix;
// using Daikon.Events.Project;

// namespace EventHistory.Infrastructure.Query.Converters
// {
//     public class EventJSONConverter : JsonConverter<BaseEvent>
//     {
//         public override bool CanConvert(Type typeToConvert)
//         {
//             return typeof(BaseEvent).IsAssignableFrom(typeToConvert);
//         }

//         public override BaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//         {
//             if (!JsonDocument.TryParseValue(ref reader, out var doc))
//             {
//                 throw new JsonException($"Failed to parse JSON. {nameof(JsonDocument)}");
//             }

//             if (!doc.RootElement.TryGetProperty("Type", out var typeProperty))
//             {
//                 throw new JsonException("Could not detect Type discriminator.");
//             }

//             var typeDiscriminator = typeProperty.GetString();
//             var json = doc.RootElement.GetRawText();

//             return typeDiscriminator switch
//             {
//                 "CommentCreatedEvent" => JsonSerializer.Deserialize<CommentCreatedEvent>(json, options),
//                 "CommentUpdatedEvent" => JsonSerializer.Deserialize<CommentUpdatedEvent>(json, options),
//                 "CommentDeletedEvent" => JsonSerializer.Deserialize<CommentDeletedEvent>(json, options),
//                 "CommentReplyAddedEvent" => JsonSerializer.Deserialize<CommentReplyAddedEvent>(json, options),
//                 "CommentReplyUpdatedEvent" => JsonSerializer.Deserialize<CommentReplyUpdatedEvent>(json, options),
//                 "CommentReplyDeletedEvent" => JsonSerializer.Deserialize<CommentReplyDeletedEvent>(json, options),
                
//                 // Ha events
//                 "HaCreatedEvent" => JsonSerializer.Deserialize<HaCreatedEvent>(json, options),
//                 "HaUpdatedEvent" => JsonSerializer.Deserialize<HaUpdatedEvent>(json, options),
//                 "HaDeletedEvent" => JsonSerializer.Deserialize<HaDeletedEvent>(json, options),
//                 "HaRenamedEvent" => JsonSerializer.Deserialize<HaRenamedEvent>(json, options),
//                 "HaCompoundEvolutionAddedEvent" => JsonSerializer.Deserialize<HaCompoundEvolutionAddedEvent>(json, options),
//                 "HaCompoundEvolutionUpdatedEvent" => JsonSerializer.Deserialize<HaCompoundEvolutionUpdatedEvent>(json, options),
//                 "HaCompoundEvolutionDeletedEvent" => JsonSerializer.Deserialize<HaCompoundEvolutionDeletedEvent>(json, options),
                
//                 // Gene events
//                 "GeneCreatedEvent" => JsonSerializer.Deserialize<GeneCreatedEvent>(json, options),
//                 "GeneUpdatedEvent" => JsonSerializer.Deserialize<GeneUpdatedEvent>(json, options),
//                 "GeneDeletedEvent" => JsonSerializer.Deserialize<GeneDeletedEvent>(json, options),
                
//                 // Molecule events
//                 "MoleculeCreatedEvent" => JsonSerializer.Deserialize<MoleculeCreatedEvent>(json, options),
//                 "MoleculeUpdatedEvent" => JsonSerializer.Deserialize<MoleculeUpdatedEvent>(json, options),
//                 "MoleculeDeletedEvent" => JsonSerializer.Deserialize<MoleculeDeletedEvent>(json, options),
                
//                 // Project events
//                 "ProjectCreatedEvent" => JsonSerializer.Deserialize<ProjectCreatedEvent>(json, options),
//                 "ProjectUpdatedEvent" => JsonSerializer.Deserialize<ProjectUpdatedEvent>(json, options),
//                 "ProjectDeletedEvent" => JsonSerializer.Deserialize<ProjectDeletedEvent>(json, options),
//                 "ProjectRenamedEvent" => JsonSerializer.Deserialize<ProjectRenamedEvent>(json, options),
//                 "ProjectCompoundEvolutionAddedEvent" => JsonSerializer.Deserialize<ProjectCompoundEvolutionAddedEvent>(json, options),
//                 "ProjectCompoundEvolutionUpdatedEvent" => JsonSerializer.Deserialize<ProjectCompoundEvolutionUpdatedEvent>(json, options),
//                 "ProjectCompoundEvolutionDeletedEvent" => JsonSerializer.Deserialize<ProjectCompoundEvolutionDeletedEvent>(json, options),

//                 _ => null,
//             };
//         }

//         public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
//         {
//             writer.WriteStartObject();
//             writer.WriteString("Type", value.GetType().Name); // Write the discriminator

//             // Serialize the object based on its actual type
//             switch (value)
//             {
//                 case CommentCreatedEvent commentCreated:
//                     JsonSerializer.Serialize(writer, commentCreated, options);
//                     break;
//                 case CommentUpdatedEvent commentUpdated:
//                     JsonSerializer.Serialize(writer, commentUpdated, options);
//                     break;
//                 case CommentDeletedEvent commentDeleted:
//                     JsonSerializer.Serialize(writer, commentDeleted, options);
//                     break;
//                 case CommentReplyAddedEvent commentReplyAdded:
//                     JsonSerializer.Serialize(writer, commentReplyAdded, options);
//                     break;
//                 case CommentReplyUpdatedEvent commentReplyUpdated:
//                     JsonSerializer.Serialize(writer, commentReplyUpdated, options);
//                     break;
//                 case CommentReplyDeletedEvent commentReplyDeleted:
//                     JsonSerializer.Serialize(writer, commentReplyDeleted, options);
//                     break;

//                 // Ha events
//                 case HaCreatedEvent haCreated:
//                     JsonSerializer.Serialize(writer, haCreated, options);
//                     break;
//                 case HaUpdatedEvent haUpdated:
//                     JsonSerializer.Serialize(writer, haUpdated, options);
//                     break;
//                 case HaDeletedEvent haDeleted:
//                     JsonSerializer.Serialize(writer, haDeleted, options);
//                     break;
//                 case HaRenamedEvent haRenamed:
//                     JsonSerializer.Serialize(writer, haRenamed, options);
//                     break;
//                 case HaCompoundEvolutionAddedEvent haCompoundEvolutionAdded:
//                     JsonSerializer.Serialize(writer, haCompoundEvolutionAdded, options);
//                     break;
//                 case HaCompoundEvolutionUpdatedEvent haCompoundEvolutionUpdated:
//                     JsonSerializer.Serialize(writer, haCompoundEvolutionUpdated, options);
//                     break;
//                 case HaCompoundEvolutionDeletedEvent haCompoundEvolutionDeleted:
//                     JsonSerializer.Serialize(writer, haCompoundEvolutionDeleted, options);
//                     break;

//                 // Gene events
//                 case GeneCreatedEvent geneCreated:
//                     JsonSerializer.Serialize(writer, geneCreated, options);
//                     break;
//                 case GeneUpdatedEvent geneUpdated:
//                     JsonSerializer.Serialize(writer, geneUpdated, options);
//                     break;
//                 case GeneDeletedEvent geneDeleted:
//                     JsonSerializer.Serialize(writer, geneDeleted, options);
//                     break;

//                 // Molecule events
//                 case MoleculeCreatedEvent moleculeCreated:
//                     JsonSerializer.Serialize(writer, moleculeCreated, options);
//                     break;
//                 case MoleculeUpdatedEvent moleculeUpdated:
//                     JsonSerializer.Serialize(writer, moleculeUpdated, options);
//                     break;
//                 case MoleculeDeletedEvent moleculeDeleted:
//                     JsonSerializer.Serialize(writer, moleculeDeleted, options);
//                     break;

//                 // Project events
//                 case ProjectCreatedEvent projectCreated:
//                     JsonSerializer.Serialize(writer, projectCreated, options);
//                     break;
//                 case ProjectUpdatedEvent projectUpdated:
//                     JsonSerializer.Serialize(writer, projectUpdated, options);
//                     break;
//                 case ProjectDeletedEvent projectDeleted:
//                     JsonSerializer.Serialize(writer, projectDeleted, options);
//                     break;
//                 case ProjectRenamedEvent projectRenamed:
//                     JsonSerializer.Serialize(writer, projectRenamed, options);
//                     break;
//                 case ProjectCompoundEvolutionAddedEvent projectCompoundEvolutionAdded:
//                     JsonSerializer.Serialize(writer, projectCompoundEvolutionAdded, options);
//                     break;
//                 case ProjectCompoundEvolutionUpdatedEvent projectCompoundEvolutionUpdated:
//                     JsonSerializer.Serialize(writer, projectCompoundEvolutionUpdated, options);
//                     break;
//                 case ProjectCompoundEvolutionDeletedEvent projectCompoundEvolutionDeleted:
//                     JsonSerializer.Serialize(writer, projectCompoundEvolutionDeleted, options);
//                     break;

//                 default:
//                     throw new JsonException($"Unknown event type: {value.GetType().Name}");
//             }

//             writer.WriteEndObject();
//         }
//     }
// }
