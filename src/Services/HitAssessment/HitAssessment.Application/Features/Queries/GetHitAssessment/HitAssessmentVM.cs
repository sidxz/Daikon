
namespace HitAssessment.Application.Features.Queries.GetHitAssessment
{
    public class HitAssessmentVM
    {
        public Guid? StrainId { get; set; }
        public string Name { get; set; }
        public string? HaType { get; set; }
        public string? LegacyId { get; set; }
        public Guid HitId { get; set; }
        public Guid CompoundId { get; set; }

        public Dictionary<string, string> AssociatedHitIds { get; set; }
        public object HAStart { get; set; }
        public object HAPredictedStart { get; set; }
        public object HADescription { get; set; }
        public object HAStatus { get; set; }

        public bool IsHAComplete { get; set; }
        public DateTime? HAStatusDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? EOLDate { get; set; }
        public DateTime? CompletionDate { get; set; }


        public object PrimaryOrg { get; set; }
        public List<string> SupportingOrgs { get; set; }

    }
}