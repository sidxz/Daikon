
using CQRS.Core.Domain;

namespace Screen.Application.Features.Queries.ViewModels
{
    public class HitVM : DocMetadata
    {
        public Guid Id { get; set; }
        public Guid HitCollectionId { get; set; }
        public object Library { get; set; }
        public object LibrarySource { get; set; }
        public object Method { get; set; }
        public object MIC { get; set; }
        public object MICUnit { get; set; }
        public object MICCondition { get; set; }
        public object IC50 { get; set; }
        public object IC50Unit { get; set; }
        public object ClusterGroup { get; set; }
        public object Notes { get; set; }

        /* Voting */
        public object Positive { get; set; }
        public object Neutral { get; set; }
        public object Negative { get; set; }
        public object IsVotingAllowed { get; set; }
        
        
       /* Molecule */
        public object RequestedSMILES { get; set; }
        public object IsStructureDisclosed { get; set; }
        public Guid MoleculeId { get; set; }
        public Guid MoleculeRegistrationId { get; set; }
    }
}