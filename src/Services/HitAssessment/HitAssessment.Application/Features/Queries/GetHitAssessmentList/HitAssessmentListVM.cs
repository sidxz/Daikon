
namespace HitAssessment.Application.Features.Queries.GetHitAssessmentList
{
    public class HitAssessmentListVM
    {
        public Guid Id { get; set; }
        public Guid? StrainId { get; set; }
        public string Name { get; set; }
        public string? HaType { get; set; }
        public string? LegacyId { get; set; }
        public object HAStart { get; set; }
        public object HAPredictedStart { get; set; }
        public object HAStatus { get; set; }

        public bool IsHAComplete { get; set; }
        public object PrimaryOrg { get; set; }
    }
}