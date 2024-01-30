
using CQRS.Core.Domain;

namespace Project.Application.Features.Queries.GetProject
{
    public class ProjectVM : DocMetadata
    {
        public Guid Id { get; set; }
        public Guid? StrainId { get; set; }
        public string Name { get; set; }
        public string? ProjectType { get; set; }
        public string? LegacyId { get; set; }
        public Guid HitId { get; set; }
        public Guid CompoundId { get; set; }

        public Dictionary<string, string> AssociatedHitIds { get; set; }
        public object ProjectStart { get; set; }
        public object ProjectPredictedStart { get; set; }
        public object ProjectDescription { get; set; }
        public object ProjectStatus { get; set; }

        public bool IsProjectComplete { get; set; }
        public DateTime? ProjectStatusDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? EOLDate { get; set; }
        public DateTime? CompletionDate { get; set; }


        public object PrimaryOrg { get; set; }
        public List<string> SupportingOrgs { get; set; }

    }
}