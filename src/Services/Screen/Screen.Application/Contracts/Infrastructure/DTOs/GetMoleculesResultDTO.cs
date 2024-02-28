using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Screen.Application.Contracts.Infrastructure.DTOs
{
    public class GetMoleculesResultDTO
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("registrationId")]
        public Guid RegistrationId { get; set; }
        [JsonPropertyName("name")]
        public object Name { get; set; }
        [JsonPropertyName("synonyms")]
        public List<string> Synonyms { get; set; }
        [JsonPropertyName("ids")]
        public Dictionary<string, string> Ids { get; set; }
        [JsonPropertyName("similarity")]
        public float Similarity { get; set; }
        [JsonPropertyName("smiles")]
        public string Smiles { get; set; }
        [JsonPropertyName("smilesCanonical")]
        public string? SmilesCanonical { get; set; }
        [JsonPropertyName("molecularWeight")]
        public float? MolecularWeight { get; set; }
        [JsonPropertyName("tpsa")]
        public float? TPSA { get; set; }
    }
}