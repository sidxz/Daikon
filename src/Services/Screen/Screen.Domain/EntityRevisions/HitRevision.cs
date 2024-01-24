
using CQRS.Core.Domain.Historical;

namespace Screen.Domain.EntityRevisions
{
    public class HitRevision : BaseVersionEntity
    {
        public DVariableHistory<string>? Library { get; set; }
        public DVariableHistory<string>? LibrarySource { get; set; }
        public DVariableHistory<string>? Method { get; set; }
        public DVariableHistory<string>? MIC { get; set; }
        public DVariableHistory<string>? MICUnit { get; set; }
        public DVariableHistory<string>? MICCondition { get; set; }
        public DVariableHistory<string>? IC50 { get; set; }
        public DVariableHistory<string>? IC50Unit { get; set; }
        public DVariableHistory<int>? ClusterGroup { get; set; }
        public DVariableHistory<string>? Notes { get; set; }

        public DVariableHistory<int>? Positive { get; set; }
        public DVariableHistory<int>? Neutral { get; set; }
        public DVariableHistory<int>? Negative { get; set; }
        public DVariableHistory<bool>? IsVotingAllowed { get; set; }

    }
}