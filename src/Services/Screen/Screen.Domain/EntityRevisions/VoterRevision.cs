

using CQRS.Core.Domain.Historical;

namespace Screen.Domain.EntityRevisions
{
    public class VoterRevision : BaseVersionEntity
    {
      
        public DVariableHistory<bool> VotedPositive { get; set; }
        public DVariableHistory<bool> VotedNeutral { get; set; }
        public DVariableHistory<bool> VotedNegative { get; set; }
        public DVariableHistory<DateTime> VotedOn { get; set; }
        public DVariableHistory<DateTime> UpdatedOn { get; set; }
        public DVariableHistory<string>? Comment { get; set; }
    }
}