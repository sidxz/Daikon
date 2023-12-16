
using CQRS.Core.Domain;

namespace Screen.Domain.Entities
{
    public class Hit : BaseEntity
    {
        public Guid HitId { get; set; }
        public Guid HitCollectionId { get; set; }
        public DVariable<string>? Library { get; set; }
        public DVariable<string>? LibrarySource { get; set; }
        public DVariable<string>? Method { get; set; }
        public DVariable<string>? MIC { get; set; }
        public DVariable<string>? MICUnit { get; set; }
        public DVariable<string>? MICCondition { get; set; }
        public DVariable<string>? IC50 { get; set; }
        public DVariable<string>? IC50Unit { get; set; }
        public DVariable<int>? ClusterGroup { get; set; }
        public DVariable<string>? Notes { get; set; }

        /* Voting */
        public DVariable<int>? Positive { get; set; }
        public DVariable<int>? Neutral { get; set; }
        public DVariable<int>? Negative { get; set; }
        public DVariable<bool>? IsVotingAllowed { get; set; }
        
        
        /* Compound */
        public DVariable<string>? InitialCompoundStructure { get; set; }
        public DVariable<string>? CompoundRegistrationStatus { get; set; }
        public DVariable<Guid>? CompoundId { get; set; }

    }
}