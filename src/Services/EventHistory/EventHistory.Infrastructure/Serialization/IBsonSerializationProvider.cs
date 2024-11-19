// using CQRS.Core.Domain;
// using MongoDB.Bson.Serialization;
// using MongoDB.Bson.Serialization.Serializers;
// using System;
// namespace EventHistory.Infrastructure.Serialization
// {
//     public class DVariableSerializationProvider : IBsonSerializationProvider
//     {
//         public IBsonSerializer GetSerializer(Type type)
//         {
//             if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DVariable<>))
//             {
//                 // Create an instance of DVariableBsonSerializer<T> where T is the generic argument
//                 var genericArgument = type.GetGenericArguments()[0];
//                 var serializerType = typeof(DVariableBsonSerializer<>).MakeGenericType(genericArgument);
//                 return (IBsonSerializer)Activator.CreateInstance(serializerType);
//             }

//             // Return null if no matching serializer is found
//             return null;
//         }
//     }
// }
