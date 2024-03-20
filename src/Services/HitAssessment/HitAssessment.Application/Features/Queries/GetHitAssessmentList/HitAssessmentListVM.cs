
namespace HitAssessment.Application.Features.Queries.GetHitAssessmentList
{
    public class HitAssessmentListVM
    {
        public Guid Id { get; set; }
        public Guid? StrainId { get; set; }
        public string Name { get; set; }
        public string? HaType { get; set; }
        public string? LegacyId { get; set; }
        public DateTime HAStart { get; set; }
        public DateTime HAPredictedStart { get; set; }
        public string? HAStatus { get; set; }

        public bool IsHAComplete { get; set; }
        public string? PrimaryOrg { get; set; }
    }
}