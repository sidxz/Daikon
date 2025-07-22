
using Daikon.EventStore.Event;

namespace Daikon.Events.MLogix
{
    public class MoleculeUpdatedEvent : BaseEvent
    {
        public MoleculeUpdatedEvent() : base(nameof(MoleculeUpdatedEvent))
        {

        }
        public Guid RegistrationId { get; set; }
        public string Name { get; set; }
        public string RequestedSMILES { get; set; }
        public string SmilesCanonical { get; set; }
        public Dictionary<string, string> Ids { get; set; }

        /* --- Disclosure Fields --- */

        // Indicates whether the molecule's structure has been disclosed
        public bool IsStructureDisclosed { get; set; }

        // Date when the molecule's structure was disclosed
        public DateTime StructureDisclosedDate { get; set; } = new DateTime();

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
        public string? DisclosureNotes { get; set; }


        // Version number of the disclosure (helps track updates/iterations of the disclosed structure)
        public int DisclosureVersion { get; set; } = 1;


        // High-level classification of disclosure type (e.g., "Public", "Confidential", "Restricted External")
        public string DisclosureType { get; set; } = "Confidential";

        /* --- Extended Disclosure Questionnaire Fields --- */

        // List of literature references (links or DOIs) supporting the molecule disclosure
        public string LiteratureReferences { get; set; } = string.Empty;


        // Number of additional undisclosed analogues related to this molecule (helps assess IP risk/coverage)
        public int? NumberOfUndisclosedAnalogues { get; set; }

        // Free text notes to provide any additional information if relevant to the disclosure answers
        public string? DisclosureAdditionalNotes { get; set; }
    }
}