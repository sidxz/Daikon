
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
        public DateTime StatusLastModifiedDate { get; set; }
        public DateTime StatusPausedDate { get; set; }
        public string? Status { get; set; }

        public Guid HitCollectionId { get; set; }
        public Guid HitId { get; set; }
        public Guid CompoundId { get; set; }

        public bool IsHAComplete { get; set; }
        public bool IsHAPromoted { get; set; }
        public bool IsHASuccess { get; set; }
        public Guid? PrimaryOrgId { get; set; }
        public List<Guid>? ParticipatingOrgsId { get; set; }
        public Guid CompoundEvoLatestMoleculeId { get; set; }
        public string CompoundEvoLatestSMILES { get; set; }
    }
}

