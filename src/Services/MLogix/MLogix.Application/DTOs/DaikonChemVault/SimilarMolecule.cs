using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MLogix.Application.DTOs.DaikonChemVault
{
    public class SimilarMolecule : MoleculeBase
    {
        [JsonPropertyName("similarity")]
        public float Similarity { get; set; }
    }
}