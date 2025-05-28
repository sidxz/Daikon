using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MLogix.Application.DTOs.DaikonChemVault
{
    public class PainsVM
    {
        [JsonPropertyName("rdkit_pains")]
        public bool RDKitPains { get; set; }

        [JsonPropertyName("rdkit_pains_label")]
        public List<string> RDKitPainsLabels { get; set; }
    }
}