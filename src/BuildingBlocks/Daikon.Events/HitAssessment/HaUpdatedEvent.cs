
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.HitAssessment
{
    public class HaUpdatedEvent : BaseEvent
    {
        public HaUpdatedEvent() : base(nameof(HaUpdatedEvent))
        {

        }

        public Guid? StrainId { get; set; }
        public string Name { get; set; }
        public string? HaType { get; set; }
        public string? LegacyId { get; set; }
        public DVariable<string> Description { get; set; }
        public DVariable<string> Status { get; set; }
        public bool IsHAComplete { get; set; }
        public bool IsHASuccess { get; set; }

        /* Associated Hit */
        public Guid HitId { get; set; }
        public Guid CompoundId { get; set; }
        public Dictionary<string, string> AssociatedHitIds { get; set; }




        /* Orgs */
        public DVariable<Guid>? PrimaryOrgId { get; set; }
        public List<Guid>? ParticipatingOrgs { get; set; }


        /* Dates */
        public DVariable<DateTime>? HaPredictedStartDate { get; set; }
        public DVariable<DateTime>? HaStartDate { get; set; }
        public DVariable<DateTime>? StatusLastModifiedDate { get; set; }
        public DVariable<DateTime>? StatusReadyForHADate { get; set; }
        public DVariable<DateTime>? StatusActiveDate { get; set; }
        public DVariable<DateTime>? StatusIncorrectMzDate { get; set; }
        public DVariable<DateTime>? StatusKnownLiabilityDate { get; set; }
        public DVariable<DateTime>? StatusCompleteFailedDate { get; set; }
        public DVariable<DateTime>? StatusCompleteSuccessDate { get; set; }

        public DVariable<DateTime>? TerminationDate { get; set; }
        public DVariable<DateTime>? CompletionDate { get; set; }
        public DVariable<DateTime>? EOLDate { get; set; }
        public DVariable<DateTime>? H2LPredictedStartDate { get; set; }
    }
}