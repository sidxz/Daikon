using System;
using System.Text.Json;

namespace CQRS.Core.Resolvers
{
    /*
     * ObjectToGuidResolver provides a utility method to safely extract a Guid value 
     * from various object types such as Guid, string, or JsonElement.
     * 
     * This class ensures type safety, null checks, and handles different input formats.
     */
    public static class ObjectToGuidResolver
    {
        /*
         * Attempts to extract a Guid from the provided object.
         * Supported input types: Guid, string (parsable to Guid), JsonElement (containing Guid as string).
         *
         * Parameters:
         *   input (object): The input object to extract the Guid from.
         *
         * Returns:
         *   Guid? - Parsed Guid if successful; otherwise, null.
         */
        public static Guid? TryExtractGuid(object input)
        {
            // Return null early if input is null
            if (input is null)
                return null;

            // Handle when input is already a Guid
            if (input is Guid directGuid)
                return directGuid;

            // Handle string input by trying to parse as Guid
            if (input is string str && Guid.TryParse(str, out var parsedFromString))
                return parsedFromString;

            // Handle JsonElement input
            if (input is JsonElement jsonElement)
            {
                // Extract if it's a string and parse it as a Guid
                if (jsonElement.ValueKind == JsonValueKind.String &&
                    Guid.TryParse(jsonElement.GetString(), out var parsedFromJson))
                {
                    return parsedFromJson;
                }

                // Explicitly handle Json nulls
                if (jsonElement.ValueKind == JsonValueKind.Null)
                    return null;
            }

            // Default: input is of unsupported type or couldn't parse a valid Guid
            return null;
        }
    }
}
