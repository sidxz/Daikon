
using CQRS.Core.Domain.Historical;

namespace HitAssessment.Domain.EntityRevisions
{
    public class HitAssessmentRevision : BaseVersionEntity
    {

        public DVariableHistory<string> Description { get; set; }
        public DVariableHistory<string> Status { get; set; }

        /* Orgs */
        public DVariableHistory<Guid>? PrimaryOrgId { get; set; }


        /* Dates */
        public DVariableHistory<DateTime>? HaPredictedStartDate { get; set; }
        public DVariableHistory<DateTime>? HaStartDate { get; set; }
        public DVariableHistory<DateTime>? StatusLastModifiedDate { get; set; }
        public DVariableHistory<DateTime>? StatusReadyForHADate { get; set; }
        public DVariableHistory<DateTime>? StatusActiveDate { get; set; }
        public DVariableHistory<DateTime>? StatusIncorrectMzDate { get; set; }
        public DVariableHistory<DateTime>? StatusKnownLiabilityDate { get; set; }
        public DVariableHistory<DateTime>? StatusCompleteFailedDate { get; set; }
        public DVariableHistory<DateTime>? StatusCompleteSuccessDate { get; set; }

        public DVariableHistory<DateTime>? RemovalDate { get; set; }
        public DVariableHistory<DateTime>? CompletionDate { get; set; }
        public DVariableHistory<DateTime>? EOLDate { get; set; }
        public DVariableHistory<DateTime>? H2LPredictedStartDate { get; set; }
    }
}