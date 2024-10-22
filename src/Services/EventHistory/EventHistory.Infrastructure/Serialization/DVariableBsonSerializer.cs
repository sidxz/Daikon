// using MongoDB.Bson;
// using MongoDB.Bson.Serialization;
// using MongoDB.Bson.Serialization.Serializers;
// using CQRS.Core.Domain;

// namespace EventHistory.Infrastructure.Serialization
// {
//     public class DVariableBsonSerializer<TDataType> : IBsonSerializer<DVariable<TDataType>>
//     {
//         public Type ValueType => typeof(DVariable<TDataType>);

//         public DVariable<TDataType> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
//         {
//             // Check if the DVariable itself is BsonNull
//             if (context.Reader.GetCurrentBsonType() == BsonType.Null)
//             {
//                 context.Reader.ReadNull();  // Consume the null value
//                 return null;  // Return null for DVariable<T> when the entire object is null
//             }

//             var document = BsonSerializer.Deserialize<BsonDocument>(context.Reader);

//             TDataType value = default;
//             if (document.Contains("Value") && !document["Value"].IsBsonNull)
//             {
//                 var valueBson = document["Value"];

//                 // Check if the value is a document or a primitive type
//                 if (valueBson is BsonDocument bsonDoc)
//                 {
//                     value = BsonSerializer.Deserialize<TDataType>(bsonDoc);
//                 }
//                 else
//                 {
//                     value = (TDataType)BsonTypeMapper.MapToDotNetValue(valueBson);
//                 }
//             }

//             return new DVariable<TDataType>
//             {
//                 Value = value,
//                 CreatedById = document.Contains("CreatedById") && !document["CreatedById"].IsBsonNull ? document["CreatedById"].AsNullableGuid : null,
//                 LastModifiedById = document.Contains("LastModifiedById") && !document["LastModifiedById"].IsBsonNull ? document["LastModifiedById"].AsNullableGuid : null,
//                 Author = document.Contains("Author") && !document["Author"].IsBsonNull ? document["Author"].AsString : null,
//                 DateCreated = document.Contains("DateCreated") && !document["DateCreated"].IsBsonNull ? document["DateCreated"].ToNullableUniversalTime() : null,
//                 DateModified = document.Contains("DateModified") && !document["DateModified"].IsBsonNull ? document["DateModified"].ToNullableUniversalTime() : null,
//                 IsModified = document.Contains("IsModified") && !document["IsModified"].IsBsonNull ? document["IsModified"].AsNullableBoolean : null,
//                 Comment = document.Contains("Comment") && !document["Comment"].IsBsonNull ? document["Comment"].AsString : null,
//                 Provenance = document.Contains("Provenance") && !document["Provenance"].IsBsonNull ? document["Provenance"].AsString : null,
//                 Source = document.Contains("Source") && !document["Source"].IsBsonNull ? document["Source"].AsString : null,
//                 IsMLGenerated = document.Contains("IsMLGenerated") && !document["IsMLGenerated"].IsBsonNull ? document["IsMLGenerated"].AsNullableBoolean : null,
//                 MLGeneratedBy = document.Contains("MLGeneratedBy") && !document["MLGeneratedBy"].IsBsonNull ? document["MLGeneratedBy"].AsString : null,
//                 MLGeneratedDate = document.Contains("MLGeneratedDate") && !document["MLGeneratedDate"].IsBsonNull ? document["MLGeneratedDate"].ToNullableUniversalTime() : null,
//                 MLGeneratedComment = document.Contains("MLGeneratedComment") && !document["MLGeneratedComment"].IsBsonNull ? document["MLGeneratedComment"].AsString : null,
//                 MLGeneratedConfidence = document.Contains("MLGeneratedConfidence") && !document["MLGeneratedConfidence"].IsBsonNull ? (float?)document["MLGeneratedConfidence"].AsNullableDouble : null,
//                 ApprovalStatus = document.Contains("ApprovalStatus") && !document["ApprovalStatus"].IsBsonNull ? document["ApprovalStatus"].AsString : null,
//                 IsVerified = document.Contains("IsVerified") && !document["IsVerified"].IsBsonNull ? document["IsVerified"].AsNullableBoolean : null,
//                 VerifiedBy = document.Contains("VerifiedBy") && !document["VerifiedBy"].IsBsonNull ? document["VerifiedBy"].AsString : null,
//                 VerifiedComment = document.Contains("VerifiedComment") && !document["VerifiedComment"].IsBsonNull ? document["VerifiedComment"].AsString : null,
//                 VerifiedDate = document.Contains("VerifiedDate") && !document["VerifiedDate"].IsBsonNull ? document["VerifiedDate"].ToNullableUniversalTime() : null,
//                 IsDraft = document.Contains("IsDraft") && !document["IsDraft"].IsBsonNull ? document["IsDraft"].AsNullableBoolean : null,
//                 DataQualityIndicator = document.Contains("DataQualityIndicator") && !document["DataQualityIndicator"].IsBsonNull ? document["DataQualityIndicator"].AsString : null
//             };
//         }

//         public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DVariable<TDataType> value)
//         {
//             if (value == null)
//             {
//                 context.Writer.WriteNull();  // Write null if the DVariable itself is null
//                 return;
//             }

//             context.Writer.WriteStartDocument();

//             context.Writer.WriteName("Value");
//             BsonSerializer.Serialize(context.Writer, typeof(TDataType), value.Value);

//             context.Writer.WriteName("CreatedById");
//             BsonSerializer.Serialize(context.Writer, typeof(Guid?), value.CreatedById);

//             context.Writer.WriteName("DateCreated");
//             BsonSerializer.Serialize(context.Writer, typeof(DateTime?), value.DateCreated);

//             context.Writer.WriteEndDocument();
//         }

//         public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
//         {
//             Serialize(context, args, (DVariable<TDataType>)value);
//         }

//         object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
//         {
//             return Deserialize(context, args);
//         }
//     }
// }