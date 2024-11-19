
using CQRS.Core.Domain;

namespace HitAssessment.Application.Features.Queries.GetHitAssessment
{
    public class HitAssessmentVM : VMMeta
    {
        public Guid Id { get; set; }
        public Guid? StrainId { get; set; }
        public string Name { get; set; }
        public string? HaType { get; set; }
        public string? LegacyId { get; set; }
        public object Description { get; set; }
        public object Status { get; set; }
        public bool IsHAComplete { get; set; }
        public bool IsHAPromoted { get; set; }
        public bool IsHASuccess { get; set; }

         /* Associated Hit */
        public Guid HitId { get; set; }
        public Guid HitCollectionId { get; set; }
        public Guid CompoundId { get; set; }
        public Dictionary<string, string> AssociatedHitIds { get; set; }
        
        /* Orgs */
        public object PrimaryOrgId { get; set; }
        public List<Guid>? ParticipatingOrgs { get; set; }

        /* Dates */
        public object HaPredictedStartDate { get; set; }
        public object HaStartDate { get; set; }
        public object StatusLastModifiedDate { get; set; }
        public object StatusReadyForHADate { get; set; }
        public object StatusActiveDate { get; set; }
        public object StatusIncorrectMzDate { get; set; }
        public object StatusKnownLiabilityDate { get; set; }
        public object StatusCompleteFailedDate { get; set; }
        public object StatusCompleteSuccessDate { get; set; }

        public object RemovalDate { get; set; }
        public object CompletionDate { get; set; }
        public object EOLDate { get; set; }
        public object H2LPredictedStartDate { get; set; }

        /* Compound Evolution */
        public List<HaCompoundEvolutionVM> HaCompoundEvolution { get; set; }
        public Guid CompoundEvoLatestMoleculeId { get; set; }
        public string CompoundEvoLatestSMILES { get; set; } // Calculated




    }
}