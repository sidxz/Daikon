
using CQRS.Core.Domain;

namespace HitAssessment.Application.Features.Queries.GetHitAssessment
{
    public class HitAssessmentVM : DocMetadata
    {
        public Guid Id { get; set; }
        public Guid? StrainId { get; set; }
        public string Name { get; set; }
        public string? HaType { get; set; }
        public string? LegacyId { get; set; }
        public Guid HitId { get; set; }
        public Guid CompoundId { get; set; }

        public Dictionary<string, string> AssociatedHitIds { get; set; }
        public object HaStartDate { get; set; }
        public object HaPredictedStartDate { get; set; }
        public object Description { get; set; }
        public object Status { get; set; }

        public bool IsHAComplete { get; set; }
        public DateTime? StatusDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? EOLDate { get; set; }
        public DateTime? CompletionDate { get; set; }


        public object PrimaryOrgId { get; set; }
        public List<Guid>? ParticipatingOrgs { get; set; }

    }
}