using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Daikon.Shared.DataStructures;

namespace Daikon.Shared.Constants.AppScreen
{
    public class VotingValue
    {
        public const string Positive = "Positive";
        public const string Negative = "Negative";
        public const string Neutral = "Neutral";
        public static List<NameValuePair> GetVotingValues()
        {
            return [.. new List<NameValuePair>
            {
                new() { Name = "Positive", Value = Positive },
                new() { Name = "Negative", Value = Negative },
                new() { Name = "Neutral", Value = Neutral },
            }.OrderBy(dv => dv.Name)];
        }
    }

}