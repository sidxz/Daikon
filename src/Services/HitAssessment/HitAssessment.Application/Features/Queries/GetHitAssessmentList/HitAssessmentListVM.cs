
using CQRS.Core.Domain;

namespace HitAssessment.Application.Features.Queries.GetHitAssessmentList
{
    public class HitAssessmentListVM : DocMetadata
    {
        public Guid Id { get; set; }
        public Guid? StrainId { get; set; }
        public string Name { get; set; }
        public string? HaType { get; set; }
        public string? LegacyId { get; set; }
        public DateTime HaStartDate { get; set; }
        public DateTime HaPredictedStartDate { get; set; }
        public string? Status { get; set; }

        public bool IsHAComplete { get; set; }
        public Guid? PrimaryOrgId { get; set; }
    }
}

