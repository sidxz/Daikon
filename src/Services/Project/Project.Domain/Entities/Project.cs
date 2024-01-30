
using CQRS.Core.Domain;

namespace Project.Domain.Entities
{
    public class Project : BaseEntity
    {
        public Guid? StrainId { get; set; }
        public string Name { get; set; }
        public string? ProjectType { get; set; }
        public string? LegacyId { get; set; }
        public Guid HitId { get; set; }
        public Guid CompoundId { get; set; }

        public Dictionary<string, string> AssociatedHitIds { get; set; }
        public DVariable<DateTime> ProjectStart { get; set; }
        public DVariable<DateTime> ProjectPredictedStart { get; set; }
        public DVariable<string> ProjectDescription { get; set; }
        public DVariable<string> ProjectStatus { get; set; }

        public bool IsProjectComplete { get; set; }
        public DateTime? ProjectStatusDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? EOLDate { get; set; }
        public DateTime? CompletionDate { get; set; }


        public DVariable<string> PrimaryOrg { get; set; }
        public List<string> SupportingOrgs { get; set; }
    }

    
}