
using CQRS.Core.Domain.Historical;

namespace HitAssessment.Domain.EntityRevisions
{
    public class HitAssessmentRevision : BaseVersionEntity
    {
        public Dictionary<string, string> AssociatedHitIds { get; set; }
        public DVariableHistory<DateTime> HAStart { get; set; }
        public DVariableHistory<DateTime> HAPredictedStart { get; set; }
        public DVariableHistory<string> HADescription { get; set; }
        public DVariableHistory<string> HAStatus { get; set; }
        public DVariableHistory<Guid> PrimaryOrgId { get; set; }
    }
}