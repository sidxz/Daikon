using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MLogix.Application.DTOs.DaikonChemVault
{
    public class RegisterMolecule
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("smiles")]
        public string Smiles { get; set; } = "";
    }
}