using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CQRS.Core.Query;

namespace MLogix.Application.Features.Queries.Filters
{
    public class BaseQueryWithConditionFilters : BaseQuery
    {
        [JsonPropertyName("molecular_weight_min")]
        public double? MolecularWeightMin { get; set; }

        [JsonPropertyName("molecular_weight_max")]
        public double? MolecularWeightMax { get; set; }

        [JsonPropertyName("clogp_min")]
        public double? cLogPMin { get; set; }

        [JsonPropertyName("clogp_max")]
        public double? cLogPMax { get; set; }

        [JsonPropertyName("lipinski_hbd_min")]
        public int? LipinskiHBDMin { get; set; }

        [JsonPropertyName("lipinski_hbd_max")]
        public int? LipinskiHBDMax { get; set; }

        [JsonPropertyName("tpsa_min")]
        public double? TpsaMin { get; set; }

        [JsonPropertyName("tpsa_max")]
        public double? TpsaMax { get; set; }

        [JsonPropertyName("rotatable_bonds_min")]
        public int? RotatableBondsMin { get; set; }

        [JsonPropertyName("rotatable_bonds_max")]
        public int? RotatableBondsMax { get; set; }

        [JsonPropertyName("heavy_atoms_min")]
        public int? HeavyAtomsMin { get; set; }

        [JsonPropertyName("heavy_atoms_max")]
        public int? HeavyAtomsMax { get; set; }

        [JsonPropertyName("aromatic_rings_min")]
        public int? AromaticRingsMin { get; set; }

        [JsonPropertyName("aromatic_rings_max")]
        public int? AromaticRingsMax { get; set; }

        [JsonPropertyName("rings_min")]
        public int? RingsMin { get; set; }

        [JsonPropertyName("rings_max")]
        public int? RingsMax { get; set; }
    }
}