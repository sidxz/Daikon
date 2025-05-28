using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MLogix.Application.DTOs.DaikonChemVault
{
    public class MoleculeBase
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("synonyms")]
        public string Synonyms { get; set; } = "";

        [JsonPropertyName("smiles")]
        public string Smiles { get; set; } = "";

        [JsonPropertyName("smiles_canonical")]
        public string SmilesCanonical { get; set; } = "";

        [JsonPropertyName("selfies")]
        public string Selfies { get; set; } = "";

        [JsonPropertyName("inchi")]
        public string Inchi { get; set; } = "";

        [JsonPropertyName("inchi_key")]
        public string InchiKey { get; set; } = "";

        [JsonPropertyName("smarts")]
        public string Smarts { get; set; } = "";

        [JsonPropertyName("o_molblock")]
        public string OMolblock { get; set; } = "";

        [JsonPropertyName("std_molblock")]
        public string StdMolblock { get; set; } = "";

        [JsonPropertyName("parent_id")]
        public Guid? ParentId { get; set; }

        [JsonPropertyName("mw")]
        public float MolecularWeight { get; set; }

        [JsonPropertyName("iupac_name")]
        public string IupacName { get; set; } = "";

        [JsonPropertyName("formula")]
        public string Formula { get; set; } = "";

        [JsonPropertyName("fsp3")]
        public float FSP3 { get; set; }

        [JsonPropertyName("n_lipinski_hba")]
        public int LipinskiHBA { get; set; }

        [JsonPropertyName("n_lipinski_hbd")]
        public int LipinskiHBD { get; set; }

        [JsonPropertyName("n_rings")]
        public int Rings { get; set; }

        [JsonPropertyName("n_hetero_atoms")]
        public int HeteroAtoms { get; set; }

        [JsonPropertyName("n_heavy_atoms")]
        public int HeavyAtoms { get; set; }

        [JsonPropertyName("n_rotatable_bonds")]
        public int RotatableBonds { get; set; }

        [JsonPropertyName("n_radical_electrons")]
        public int RadicalElectrons { get; set; }

        [JsonPropertyName("tpsa")]
        public float TPSA { get; set; }

        [JsonPropertyName("qed")]
        public float QED { get; set; }

        [JsonPropertyName("clogp")]
        public float CLogP { get; set; }

        [JsonPropertyName("sas")]
        public float SAS { get; set; }

        [JsonPropertyName("n_aliphatic_carbocycles")]
        public int AliphaticCarbocycles { get; set; }

        [JsonPropertyName("n_aliphatic_heterocycles")]
        public int AliphaticHeterocycles { get; set; }

        [JsonPropertyName("n_aliphatic_rings")]
        public int AliphaticRings { get; set; }

        [JsonPropertyName("n_aromatic_carbocycles")]
        public int AromaticCarbocycles { get; set; }

        [JsonPropertyName("n_aromatic_heterocycles")]
        public int AromaticHeterocycles { get; set; }

        [JsonPropertyName("n_aromatic_rings")]
        public int AromaticRings { get; set; }

        [JsonPropertyName("n_saturated_carbocycles")]
        public int SaturatedCarbocycles { get; set; }

        [JsonPropertyName("n_saturated_heterocycles")]
        public int SaturatedHeterocycles { get; set; }

        [JsonPropertyName("n_saturated_rings")]
        public int SaturatedRings { get; set; }

        [JsonPropertyName("ro5_compliant")]
        public bool RO5Compliant { get; set; }

        [JsonPropertyName("pains")]
        public PainsVM Pains { get; set; }
    }
}