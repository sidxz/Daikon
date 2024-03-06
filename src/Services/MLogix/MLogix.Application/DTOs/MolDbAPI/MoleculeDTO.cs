
using System.Text.Json.Serialization;

namespace MLogix.Application.DTOs.MolDbAPI
{
    public class MoleculeDTO
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }


        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("similarity")]
        public float? Similarity { get; set; }

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