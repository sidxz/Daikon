

using CQRS.Core.Domain.Historical;

namespace Screen.Domain.EntityRevisions
{
    public class ScreenRevision : BaseVersionEntity
    {
        
        public DVariableHistory<string>? Method { get; set; }
        public DVariableHistory<string>? Status { get; set; }
        public DVariableHistory<string>? Notes { get; set; }
        public DVariableHistory<Guid>? PrimaryOrgId { get; set; }
        public DVariableHistory<string>? PrimaryOrgName { get; set; }
        public DVariableHistory<DateTime>? ExpectedCompleteDate { get; set; }

        
    }
}