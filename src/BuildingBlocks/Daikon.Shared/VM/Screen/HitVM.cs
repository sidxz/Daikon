using CQRS.Core.Domain;
using Daikon.Shared.Embedded.Screens;
using Daikon.Shared.VM.MLogix;
using System;
using System.Collections.Generic;

namespace Daikon.Shared.VM.Screen
{
    public class HitVM : VMMeta
    {
        public Guid Id { get; set; }
        public Guid HitCollectionId { get; set; }

        /* Library Information */
        public object Library { get; set; }
        public object LibrarySource { get; set; }
        public object Method { get; set; }

        /* Assay */
        public string AssayType { get; set; }

        /* Standard Measurements */
        public object IC50 { get; set; }
        public object IC50Unit { get; set; }
        public object EC50 { get; set; }
        public object EC50Unit { get; set; }
        public object Ki { get; set; }
        public object KiUnit { get; set; }
        public object Kd { get; set; }
        public object KdUnit { get; set; }
        public object MIC { get; set; }
        public object MICUnit { get; set; }
        public object MICCondition { get; set; }
        public object MIC90 { get; set; }
        public object MIC90Unit { get; set; }
        public object MIC90Condition { get; set; }
        public object GI50 { get; set; }
        public object GI50Unit { get; set; }
        public object TGI { get; set; }
        public object TGIUnit { get; set; }
        public object LD50 { get; set; }
        public object LD50Unit { get; set; }
        public object PctInhibition { get; set; }
        public object PctInhibitionConcentration { get; set; }
        public object PctInhibitionConcentrationUnit { get; set; }

        /* Full Dose-Response Data */
        public List<DoseResponse> DoseResponses { get; set; } = new();

        /* Grouping and Notes */
        public object ClusterGroup { get; set; }
        public object Notes { get; set; }

        /* Voting System */
        public object Positive { get; set; }
        public object Neutral { get; set; }
        public object Negative { get; set; }
        public object IsVotingAllowed { get; set; }
        public Dictionary<string, string> Voters { get; set; } = new();

        /* Voting Derived Fields */
        public string UsersVote { get; set; }
        public int VoteScore { get; set; }

        /* Molecule Metadata */
        public object RequestedSMILES { get; set; }
        public object IsStructureDisclosed { get; set; }
        public string RequestedMoleculeName { get; set; }
        public Guid MoleculeId { get; set; }
        public Guid MoleculeRegistrationId { get; set; }

        /* Related Molecule Info */
        public MoleculeVM Molecule { get; set; }
    }
}
