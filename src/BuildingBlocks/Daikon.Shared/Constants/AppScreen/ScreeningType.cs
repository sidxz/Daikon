using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.Shared.DataStructures;

namespace Daikon.Shared.Constants.AppScreen
{
    public class ScreeningType
    {
        public const string TargetBased = "target-based";
        public const string Phenotypic = "phenotypic";
        public static List<NameValuePair> GetScreeningTypes()
        {
            return [.. new List<NameValuePair>
            {
                new() { Name = "Target Based", Value = TargetBased },
                new() { Name = "Phenotypic", Value = Phenotypic },
            }.OrderBy(dv => dv.Name)];
        }
    }
}