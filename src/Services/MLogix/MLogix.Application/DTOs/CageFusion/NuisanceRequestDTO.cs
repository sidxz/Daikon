using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MLogix.Application.DTOs.CageFusion
{
    public class NuisanceRequestTuple
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("smiles")]
        public string SMILES { get; set; } = string.Empty;
    }
    public class NuisanceRequestDTO
    {
        [JsonPropertyName("items")]
        public List<NuisanceRequestTuple> Items { get; set; } = [];


        [JsonPropertyName("plot_all_attention")]
        public bool PlotAllAttention { get; set; } = false;

    }
}