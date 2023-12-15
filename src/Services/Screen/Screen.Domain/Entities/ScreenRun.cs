
using CQRS.Core.Domain;

namespace Screen.Domain.Entities
{
    public class ScreenRun : BaseEntity
    {
        public Guid ScreenId { get; set; }
        
        public DVariable<string> Library { get; set; }
        public DVariable<string>? Protocol { get; set; }
        public DVariable<int>? LibrarySize { get; set; }
        public DVariable<string>? Scientist { get; set; }
        public DVariable<DateTime>? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DVariable<int>? UnverifiedHitCount { get; set; }
        public DVariable<double>? HitRate { get; set; }
        public DVariable<int>? PrimaryHitCount { get; set; }
        public DVariable<int>? ConfirmedHitCount { get; set; }
        public DVariable<int>? NoOfCompoundsScreened { get; set; }
        public DVariable<string>? Concentration { get; set; }
        public DVariable<string>? ConcentrationUnit { get; set; }
        public DVariable<string>? Notes { get; set; }
    }
}