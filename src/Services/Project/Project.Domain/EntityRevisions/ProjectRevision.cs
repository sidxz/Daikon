
using CQRS.Core.Domain.Historical;

namespace Project.Domain.EntityRevisions
{
    public class ProjectRevision : BaseVersionEntity
    {
        public Dictionary<string, string> AssociatedHitIds { get; set; }
        public DVariableHistory<DateTime> ProjectStart { get; set; }
        public DVariableHistory<DateTime> ProjectPredictedStart { get; set; }
        public DVariableHistory<string> ProjectDescription { get; set; }
        public DVariableHistory<string> ProjectStatus { get; set; }
        public DVariableHistory<string> PrimaryOrg { get; set; }
    }
}