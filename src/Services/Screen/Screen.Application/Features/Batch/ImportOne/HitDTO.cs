
using CQRS.Core.Domain;

namespace Screen.Application.Features.Batch.ImportOne
{
    public class HitDTO : BaseEntity
    {
        public Guid HitCollectionId { get; set; }
        public string? Library { get; set; }
        public string? LibrarySource { get; set; }
        public string? Method { get; set; }
        public string? MIC { get; set; }
        public string? MICUnit { get; set; }
        public string? MICCondition { get; set; }
        public string? IC50 { get; set; }
        public string? IC50Unit { get; set; }
        public int? ClusterGroup { get; set; }
        public string? Notes { get; set; }

        /* Voting */
        public int? Positive { get; set; }
        public int? Neutral { get; set; }
        public int? Negative { get; set; }
        public bool? IsVotingAllowed { get; set; }

        // userId, voting value
        public Dictionary<string, string>? Voters { get; set; }


        /* Molecule */
        public string? RequestedSMILES { get; set; }
        public string? MoleculeName { get; set; }
        public bool IsStructureDisclosed { get; set; }
        public Guid? MoleculeId { get; set; }
        public Guid? MoleculeRegistrationId { get; set; }
    }
}