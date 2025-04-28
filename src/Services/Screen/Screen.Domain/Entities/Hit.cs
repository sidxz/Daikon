
using CQRS.Core.Domain;
using Daikon.Shared.Embedded.Screens;

namespace Screen.Domain.Entities
{
    public class Hit : BaseEntity
    {
        public Guid HitCollectionId { get; set; }

        

        /* Name of the library from which this hit originated (e.g., "Prestwick", "Enamine"). */
        public DVariable<string>? Library { get; set; }

        /* Source or provider of the library (e.g., "Commercial", "In-house Synthesis"). */
        public DVariable<string>? LibrarySource { get; set; }

        /* Method used to identify this hit (e.g., "HTS", "Virtual Screening", "FRET Assay"). */
        public DVariable<string>? Method { get; set; }

        /* Broad classification of assay type used.
         * Examples:
         * - "Enzyme"
         * - "WholeCell"
         * - "Binding"
         * - "Phenotypic"
         */
        public string? AssayType { get; set; }

        /* --- Standard Measurement Fields (Single Values) --- */
        /* Concentration causing 50% inhibition of biological activity. */
        public DVariable<string>? IC50 { get; set; }

        /* Unit for IC50 value, usually "nM" or "μM". */
        public DVariable<string>? IC50Unit { get; set; }

        /* Concentration causing 50% activation of biological activity. */
        public DVariable<string>? EC50 { get; set; }

        /* Unit for EC50 value, usually "nM" or "μM". */
        public DVariable<string>? EC50Unit { get; set; }

        /* Inhibition constant; binding affinity of an inhibitor to a target (lower = stronger binding). */
        public DVariable<string>? Ki { get; set; }

        /* Unit for Ki value, usually "nM" or "μM". */
        public DVariable<string>? KiUnit { get; set; }

        /* Dissociation constant; general strength of binding interaction (lower = tighter binding). */
        public DVariable<string>? Kd { get; set; }

        /* Unit for Kd value, usually "nM" or "μM". */
        public DVariable<string>? KdUnit { get; set; }

        /* Minimum Inhibitory Concentration; lowest concentration preventing visible growth (used in antimicrobial screens). */
        public DVariable<string>? MIC { get; set; }

        /* Unit for MIC value, usually "μg/mL" or "μM". */
        public DVariable<string>? MICUnit { get; set; }
        public DVariable<string>? MICCondition { get; set; }

        /* MIC90; concentration at which 90% inhibition is observed (common in microbiology). */
        public DVariable<string>? MIC90 { get; set; }

        /* Unit for MIC90 value, usually "μg/mL" or "μM". */
        public DVariable<string>? MIC90Unit { get; set; }

        /* Experimental condition for MIC90 value.
         * Example values: "24h", "48h", "72h" (duration of exposure).
         */
        public DVariable<string>? MIC90Condition { get; set; }

        /* Concentration that inhibits 50% of cell growth (common in cancer research). */
        public DVariable<string>? GI50 { get; set; }

        /* Unit for GI50 value, usually "nM" or "μM". */
        public DVariable<string>? GI50Unit { get; set; }

        /* Total Growth Inhibition; concentration that completely stops cell proliferation. */
        public DVariable<string>? TGI { get; set; }

        /* Unit for TGI value, usually "nM" or "μM". */
        public DVariable<string>? TGIUnit { get; set; }

        /* Lethal Dose 50; concentration or dose that kills 50% of cells or organisms. */
        public DVariable<string>? LD50 { get; set; }

        /* Unit for LD50 value.
         * Typical units:
         * - "μM" for in vitro
         * - "mg/kg" for in vivo studies.
         */
        public DVariable<string>? LD50Unit { get; set; }

        /* Full dose-response measurements if available.
         * Used for plotting curves or re-fitting IC50/EC50 values.
         */
        public List<DoseResponse>? DoseResponses { get; set; } = [];



        /* --- Grouping and Free-Text Notes --- */

        /* Cluster Group Identifier.
         * Groups related hits (e.g., similar scaffold, chemical series).
         * Optional — used during clustering analysis.
         */
        public DVariable<int>? ClusterGroup { get; set; }



        /* General comments, notes, or manual annotations related to this hit. */
        public DVariable<string>? Notes { get; set; }



        /* --- Voting System for Team Review --- */
        public DVariable<int>? Positive { get; set; }
        public DVariable<int>? Neutral { get; set; }
        public DVariable<int>? Negative { get; set; }
        public DVariable<bool>? IsVotingAllowed { get; set; }

         /* Voters dictionary.
         * Key = user ID
         * Value = voting value ("Positive", "Neutral", "Negative").
         */
        public Dictionary<string, string>? Voters { get; set; }

        /* --- Molecule Metadata (Structure / Linking) --- */
        public DVariable<string>? RequestedSMILES { get; set; }
        public string? RequestedMoleculeName { get; set; }
        public DVariable<bool> IsStructureDisclosed { get; set; }
        public Guid? MoleculeId { get; set; }
        public Guid? MoleculeRegistrationId { get; set; }

    }
}