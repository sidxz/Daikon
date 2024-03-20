
using Daikon.Shared.DataStructures;

namespace Daikon.Shared.Constants.AppTarget
{
    public class TargetType
    {
        public const string Protein = "protein";
        public const string ProteinComplex = "protein-complex";
        public static List<NameValuePair> GetScreeningTypes()
        {
            return [.. new List<NameValuePair>
            {
                new() { Name = "Protein", Value = Protein },
                new() { Name = "Protein Complex", Value = ProteinComplex },
            }.OrderBy(dv => dv.Name)];
        }
    }
}