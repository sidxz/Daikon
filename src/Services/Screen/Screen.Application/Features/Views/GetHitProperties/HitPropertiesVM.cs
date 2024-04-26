using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using Screen.Application.Features.Queries.ViewModels;

namespace Screen.Application.Features.Views.GetHitProperties
{
    public class HitPropertiesVM : DocMetadata
    {
        public Guid Id { get; set; }
        public Guid HitCollectionId { get; set; }
        public Guid ScreenId { get; set; }
        public string ScreenName { get; set; }
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
        // public object Positive { get; set; }
        // public object Neutral { get; set; }
        // public object Negative { get; set; }
        // public object IsVotingAllowed { get; set; }
        // public Dictionary<string, string>? Voters { get; set; }

        /* Voting Derived */
        // public string UsersVote { get; set; }
        // public int VoteScore { get; set; }


        /* Molecule */
        public object RequestedSMILES { get; set; }
        public object IsStructureDisclosed { get; set; }
        public Guid MoleculeId { get; set; }
        public Guid MoleculeRegistrationId { get; set; }

        // public MoleculeVM Molecule { get; set; }
    }
}