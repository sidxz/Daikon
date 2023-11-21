using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CQRS.Core.Domain;

/* 

Custom JSON converter for DVariable<T>
== Overview ==
The converter allows JSON deserialization to support two formats:
1. Direct value assignment
{
  "DVarName": "abc"
}

2. Object with properties
OR
{
  "DVarName": { "Value": "abc", "OtherMetaProperty": "xyz" }
}

== Design Considerations ==
The DVariableJsonConverter is designed to provide flexibility in JSON deserialization.
It handles two cases: direct value assignment and object with properties.
This design allows API consumers to use either format for providing data, making the 
API more adaptable to different client needs.
*/

namespace CQRS.Core.Converters
{
    public class DVariableJsonConverter<T> : JsonConverter<DVariable<T>>
    {
        public override DVariable<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // If the token is StartObject, deserialize as DVariable<T>
                return JsonSerializer.Deserialize<DVariable<T>>(ref reader, options);
            }
            else
            {
                // Otherwise, deserialize the value directly
                var value = JsonSerializer.Deserialize<T>(ref reader, options);
                return new DVariable<T> { Value = value };
            }
        }

        public override void Write(Utf8JsonWriter writer, DVariable<T> value, JsonSerializerOptions options)
        {
            // Serialize only the Value property
            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }
}