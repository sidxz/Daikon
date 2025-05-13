using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Daikon.Shared.VM.MLogix
{
    public class ClusterVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        [JsonPropertyName("smiles")]
        public string SMILES { get; set; }
        public int Cluster { get; set; }
        public bool Centroid { get; set; }
    }
}