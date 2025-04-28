
using CQRS.Core.Domain;
using Daikon.EventStore.Event;
using Daikon.Shared.Embedded.Screens;

namespace Daikon.Events.Screens
{
    public class HitUpdatedEvent : BaseEvent
    {
        public HitUpdatedEvent() : base(nameof(HitUpdatedEvent))
        {

        }

        /* Identity */
        public Guid HitId { get; set; }

        /* Library Information */
        public DVariable<string>? Library { get; set; }
        public DVariable<string>? LibrarySource { get; set; }
        public DVariable<string>? Method { get; set; }

        /* Assay */
        public string? AssayType { get; set; }
        /* Standard Measurements */
        public DVariable<string>? IC50 { get; set; }
        public DVariable<string>? IC50Unit { get; set; }
        public DVariable<string>? EC50 { get; set; }
        public DVariable<string>? EC50Unit { get; set; }
        public DVariable<string>? Ki { get; set; }
        public DVariable<string>? KiUnit { get; set; }
        public DVariable<string>? Kd { get; set; }
        public DVariable<string>? KdUnit { get; set; }
        public DVariable<string>? MIC { get; set; }
        public DVariable<string>? MICUnit { get; set; }
        public DVariable<string>? MICCondition { get; set; }
        public DVariable<string>? MIC90 { get; set; }
        public DVariable<string>? MIC90Unit { get; set; }
        public DVariable<string>? MIC90Condition { get; set; }
        public DVariable<string>? GI50 { get; set; }
        public DVariable<string>? GI50Unit { get; set; }
        public DVariable<string>? TGI { get; set; }
        public DVariable<string>? TGIUnit { get; set; }
        public DVariable<string>? LD50 { get; set; }
        public DVariable<string>? LD50Unit { get; set; }

        /* Full Dose-Response Data */
        public List<DoseResponse>? DoseResponses { get; set; }

        /* Grouping and Notes */
        public DVariable<int>? ClusterGroup { get; set; }
        public DVariable<string>? Notes { get; set; }


        /* Voting System */
        public DVariable<bool>? IsVotingAllowed { get; set; }
        public Tuple<string, string>? VoteToAdd { get; set; }

        /* Molecule Metadata */

        /* Updates to below are not allowed */
        /*
        public DVariable<string>? RequestedSMILES { get; set; }
        public string? RequestedMoleculeName { get; set; }
        public DVariable<bool> IsStructureDisclosed { get; set; }
        public Guid? MoleculeId { get; set; }
        public Guid? MoleculeRegistrationId { get; set; }
        */
    }
}