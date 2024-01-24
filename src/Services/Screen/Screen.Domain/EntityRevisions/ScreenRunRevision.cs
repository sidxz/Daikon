

using CQRS.Core.Domain.Historical;

namespace Screen.Domain.EntityRevisions
{
    public class ScreenRunRevision : BaseVersionEntity
    {
        
        public DVariableHistory<string> Library { get; set; }
        public DVariableHistory<string>? Protocol { get; set; }
        public DVariableHistory<string>? LibrarySize { get; set; }
        public DVariableHistory<Guid>? Scientist { get; set; }
        public DVariableHistory<DateTime>? StartDate { get; set; }
        public DVariableHistory<DateTime>? EndDate { get; set; }
        public DVariableHistory<int>? UnverifiedHitCount { get; set; }
        public DVariableHistory<double>? HitRate { get; set; }
        public DVariableHistory<int>? PrimaryHitCount { get; set; }
        public DVariableHistory<int>? ConfirmedHitCount { get; set; }
        public DVariableHistory<int>? NoOfCompoundsScreened { get; set; }
        public DVariableHistory<string>? Concentration { get; set; }
        public DVariableHistory<string>? ConcentrationUnit { get; set; }
        public DVariableHistory<string>? Notes { get; set; }

        
    }
}