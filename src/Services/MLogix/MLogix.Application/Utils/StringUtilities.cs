namespace MLogix.Application.Utils
{
    public class StringUtilities
    {
        /*
         * Parses a comma-separated string into a cleaned list of strings.
         * Trims whitespace and removes empty entries.
         */
        public static List<string> ExtractSynonyms(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new List<string>();

            return input
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }


        
    }
}