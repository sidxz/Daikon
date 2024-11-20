

using CQRS.Core.Domain;

namespace Screen.Domain.Entities
{
    public class Screen : BaseEntity
    {
        public Guid? StrainId { get; set; }
        public required string Name { get; set; }
        public Dictionary<string, string>? AssociatedTargets { get; set; }
        public string? ScreenType { get; set; }
        public DVariable<string>? Method { get; set; }
        public DVariable<string>? Status { get; set; }
        public DVariable<DateTime>? LatestStatusChangeDate { get; set; }
        public DVariable<string>? Notes { get; set; }
        public DVariable<Guid>? PrimaryOrgId { get; set; }
        public DVariable<string>? PrimaryOrgName { get; set; }

        public string? Owner { get; set; }
        public DVariable<DateTime>? ExpectedStartDate { get; set; }
        public DVariable<DateTime>? ExpectedCompleteDate { get; set; }

        public DVariable<DateTime>? ActualStartDate { get; set; }
        public DVariable<DateTime>? ActualCompleteDate { get; set; }
        public Dictionary<string, string>? ParticipatingOrgs { get; set; }
    }
}