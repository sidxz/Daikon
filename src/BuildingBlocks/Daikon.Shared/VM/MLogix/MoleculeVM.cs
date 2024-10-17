
using CQRS.Core.Domain;

namespace Daikon.Shared.VM.MLogix
{
    public class MoleculeVM : VMMeta
    {
        public Guid Id { get; set; }
        public Guid RegistrationId { get; set; }
        public string Name { get; set; }
        public string Synonyms { get; set; }
        public string Smiles { get; set; }
        public string SmilesCanonical { get; set; }
        public string Selfies { get; set; }
        public string Inchi { get; set; }
        public string InchiKey { get; set; }
        public string Smarts { get; set; }
        public string OMolblock { get; set; }
        public string StdMolblock { get; set; }
        public Guid? ParentId { get; set; }
        public float MolecularWeight { get; set; }
        public string IupacName { get; set; }
        public string Formula { get; set; }
        public float FSP3 { get; set; }
        public int LipinskiHBA { get; set; }
        public int LipinskiHBD { get; set; }
        public int Rings { get; set; }
        public int HeteroAtoms { get; set; }
        public int HeavyAtoms { get; set; }
        public int RotatableBonds { get; set; }
        public int RadicalElectrons { get; set; }
        public float TPSA { get; set; }
        public float QED { get; set; }
        public float CLogP { get; set; }
        public float SAS { get; set; }
        public int AliphaticCarbocycles { get; set; }
        public int AliphaticHeterocycles { get; set; }
        public int AliphaticRings { get; set; }
        public int AromaticCarbocycles { get; set; }
        public int AromaticHeterocycles { get; set; }
        public int AromaticRings { get; set; }
        public int SaturatedCarbocycles { get; set; }
        public int SaturatedHeterocycles { get; set; }
        public int SaturatedRings { get; set; }
        public bool RO5Compliant { get; set; }
    }
}