using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.Shared.DataStructures;

namespace Daikon.Shared.Constants.AppScreen
{
    public class ScreeningMethod
    {
        public const string Biochemical = "biochemical";
        public const string DnaEncodedDel = "dna-encoded-del";
        public const string Fragment = "fragment";
        public const string Other = "other";
        public const string Hypomorph = "hypomorph";
        public const string Virtual = "virtual";
        public const string Affinity = "affinity";

        public static List<NameValuePair> GetScreeningMethods()
        {
            return [.. new List<NameValuePair>
            {
                new() { Name = "Biochemical", Value = Biochemical },
                new() { Name = "DNA Encoded Del", Value = DnaEncodedDel },
                new() { Name = "Fragment", Value = Fragment },
                new() { Name = "Other", Value = Other },
                new() { Name = "Hypomorph", Value = Hypomorph },
                new() { Name = "Virtual", Value = Virtual },
                new() { Name = "Affinity", Value = Affinity },
            }.OrderBy(dv => dv.Name)];
        }
    }
}