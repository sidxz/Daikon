using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.Shared.DataStructures;

namespace Daikon.Shared.Constants.Workflow
{
    public class Stages
    {
        public const string Gene = "Gene";
        public const string Target = "Target";
        public const string Screen = "Screen";
        public const string HA = "HA";
        public const string H2L = "H2L";
        public const string LO = "LO";
        public const string SP = "SP";

        public const string IND = "IND";
        public const string P1 = "P1";
        public const string Portfolio = "Portfolio";
        public const string PostPortfolio = "PostPortfolio";
        public static List<NameValuePair> GetStages()
        {
            return [.. new List<NameValuePair>
            {
                new() { Name = "Gene", Value = Gene },
                new() { Name = "Target", Value = Target },
                new() { Name = "HA", Value = HA },
                new() { Name = "H2L", Value = H2L },
                new() { Name = "LO", Value = LO },
                new() { Name = "SP", Value = SP },
                new() { Name = "IND", Value = IND },
                new() { Name = "P1", Value = P1 }
            }.OrderBy(dv => dv.Name)];
        }
    }
}