using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.Shared.DataStructures;

namespace Daikon.Shared.Constants.AppProject
{
    public class ProjectStage
    {
        public const string H2L = "H2L";
        public const string LO = "LO";
        public const string SP = "SP";
        public const string IND = "IND";
        public const string P1 = "P1";

        public static List<NameValuePair> GetProjectStages()
        {
            return [.. new List<NameValuePair>
            {
                new() { Name = H2L, Value = nameof(H2L) },
                new() { Name = LO, Value = nameof(LO) },
                new() { Name = SP, Value = nameof(SP) },
                new() { Name = IND, Value = nameof(IND) },
                new() { Name = P1, Value = nameof(P1) },
              
            }];
        }
    }
}