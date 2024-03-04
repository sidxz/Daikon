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
        public const string Positive = "positive";
        public const string Negative = "negative";
        public const string Neutral = "neutral";
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