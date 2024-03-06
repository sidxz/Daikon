
using CQRS.Core.Domain;

namespace Screen.Domain.Entities
{
    public class ScreenRun : BaseEntity
    {
        public Guid ScreenId { get; set; }

        public DVariable<string> Library { get; set; }
        public DVariable<string>? Protocol { get; set; }
        public DVariable<string>? LibrarySize { get; set; }
        public DVariable<string>? Scientist { get; set; }
        public DVariable<DateTime>? StartDate { get; set; }
        public DVariable<DateTime>? EndDate { get; set; }
        public DVariable<string>? UnverifiedHitCount { get; set; }
        public DVariable<string>? HitRate { get; set; }
        public DVariable<string>? PrimaryHitCount { get; set; }
        public DVariable<string>? ConfirmedHitCount { get; set; }
        public DVariable<string>? NoOfCompoundsScreened { get; set; }
        public DVariable<string>? Concentration { get; set; }
        public DVariable<string>? ConcentrationUnit { get; set; }
        public DVariable<string>? Notes { get; set; }
    }
}