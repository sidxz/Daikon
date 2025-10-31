using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MLogix.Application.DTOs.CageFusion
{
    public class NuisanceResponseRow
    {
        [JsonPropertyName("Original Index")]
        public int OriginalIndex { get; set; }

        [JsonPropertyName("Id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("SMILES")]
        public string SMILES { get; set; } = string.Empty;

        [JsonPropertyName("pred_class_aggregator")]
        public int PredClassAggregator { get; set; } = 0;

        [JsonPropertyName("aggregator")]
        public double Aggregator { get; set; } = 0.0;

        [JsonPropertyName("pred_class_luciferase_inhibitor")]
        public int PredClassLuciferaseInhibitor { get; set; } = 0;

        [JsonPropertyName("luciferase_inhibitor")]
        public double LuciferaseInhibitor { get; set; } = 0.0;

        [JsonPropertyName("pred_class_reactive")]
        public int PredClassReactive { get; set; } = 0;

        [JsonPropertyName("reactive")]
        public double Reactive { get; set; } = 0.0;

        [JsonPropertyName("pred_class_promiscuous")]
        public int PredClassPromiscuous { get; set; } = 0;

        [JsonPropertyName("promiscuous")]
        public double Promiscuous { get; set; } = 0.0;
    }
    public class NuisanceResponseDTO
    {
        [JsonPropertyName("model_name")]
        public string ModelName { get; set; } = string.Empty;
        [JsonPropertyName("time_generated")]
        public DateTime TimeGenerated { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("n")]
        public int N { get; set; } = 0;

        [JsonPropertyName("columns")]
        public List<string> Columns { get; set; } = new List<string>();

        [JsonPropertyName("rows")]
        public List<NuisanceResponseRow> Rows { get; set; } = [];
    }
}