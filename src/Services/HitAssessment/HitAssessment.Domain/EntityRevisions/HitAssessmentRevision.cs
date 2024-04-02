
using CQRS.Core.Domain.Historical;

namespace HitAssessment.Domain.EntityRevisions
{
    public class HitAssessmentRevision : BaseVersionEntity
    {
        public Dictionary<string, string> AssociatedHitIds { get; set; }
        public DVariableHistory<DateTime> HaStartDate { get; set; }
        public DVariableHistory<DateTime> HaPredictedStartDate { get; set; }
        public DVariableHistory<string> Description { get; set; }
        public DVariableHistory<string> Status { get; set; }
        public DVariableHistory<Guid> PrimaryOrgId { get; set; }
    }
}