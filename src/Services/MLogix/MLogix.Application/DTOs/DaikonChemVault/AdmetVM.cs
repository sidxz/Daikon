using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MLogix.Application.DTOs.DaikonChemVault
{
    public class AdmetVM
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "";

        [JsonPropertyName("predictions")]
        public Dictionary<string, object> Predictions { get; set; }

        [JsonPropertyName("model_version")]
        public string ModelVersion { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }
    }

    public class AdmetCalcResult
    {
        [JsonPropertyName("smiles")]
        public string Smiles { get; set; } = "";

        [JsonPropertyName("predictions")]
        public Dictionary<string, object> Predictions { get; set; }

        [JsonPropertyName("model_version")]
        public string ModelVersion { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }
    }

    public class AdmetBackfillCountsVM
    {
        [JsonPropertyName("total_molecules")]
        public int TotalMolecules { get; set; }

        [JsonPropertyName("done")]
        public int Done { get; set; }

        [JsonPropertyName("pending")]
        public int Pending { get; set; }

        [JsonPropertyName("error")]
        public int Error { get; set; }

        [JsonPropertyName("missing")]
        public int Missing { get; set; }
    }

    public class AdmetBackfillTriggerVM
    {
        [JsonPropertyName("queued")]
        public bool Queued { get; set; }

        [JsonPropertyName("chunk_size")]
        public int ChunkSize { get; set; }

        [JsonPropertyName("limit")]
        public int? Limit { get; set; }

        [JsonPropertyName("include_errors")]
        public bool IncludeErrors { get; set; }

        [JsonPropertyName("pre_counts")]
        public AdmetBackfillCountsVM PreCounts { get; set; }
    }
}
