
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
        public PainsVM Pains { get; set; }


        /* --- Disclosure Fields --- */

        // Indicates whether the molecule's structure has been disclosed
        public bool IsStructureDisclosed { get; set; }

        // Date when the molecule's structure was disclosed
        public DateTime? StructureDisclosedDate { get; set; } 

        // Name of the scientist who performed the disclosure
        public string DisclosureScientist { get; set; } = string.Empty;

        // Organization ID of the entity that disclosed the structure
        public Guid DisclosureOrgId { get; set; } = Guid.Empty;

        // Reason for disclosing the structure (e.g., "collaboration", "publication", "patent filing")
        public string DisclosureReason { get; set; } = string.Empty;

        // Source of disclosure (e.g., "internal database", "publication DOI", "patent number")
        public string DisclosureSource { get; set; } = string.Empty;

        // Stage at which the structure was disclosed (e.g., "Lead", "Preclinical", "Clinical")
        public string DisclosureStage { get; set; } = string.Empty;

        // Free text for additional comments, context, or caveats about the disclosure
        public string? DisclosureNotes { get; set; } = string.Empty;

        
        // Version number of the disclosure (helps track updates/iterations of the disclosed structure)
        public int DisclosureVersion { get; set; } = 1;

        
        // High-level classification of disclosure type (e.g., "Public", "Confidential", "Restricted External")
        public string DisclosureType { get; set; } = "Confidential";

        // Internal user ID of the person who disclosed the structure (for audit tracking)
        public Guid StructureDisclosedByUserId { get; set; }

        /* --- Extended Disclosure Questionnaire Fields --- */

        // List of literature references (links or DOIs) supporting the molecule disclosure
        public string LiteratureReferences { get; set; } = string.Empty;


        // Number of additional undisclosed analogues related to this molecule (helps assess IP risk/coverage)
        public int? NumberOfUndisclosedAnalogues { get; set; }

        // Free text notes to provide any additional information if relevant to the disclosure answers
        public string? DisclosureAdditionalNotes { get; set; }
    }
}