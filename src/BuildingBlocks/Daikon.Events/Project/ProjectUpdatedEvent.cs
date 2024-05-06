
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Project
{
    public class ProjectUpdatedEvent : BaseEvent
    {
        public ProjectUpdatedEvent() : base(nameof(ProjectUpdatedEvent))
        {

        }


        public string Alias { get; set; }
        public string? ProjectType { get; set; }
        public string? LegacyId { get; set; }
        public DVariable<string> Description { get; set; }
        public DVariable<string> H2LDescription { get; set; }
        public DVariable<string> LODescription { get; set; }
        public DVariable<string> SPDescription { get; set; }
        public DVariable<string> INDDescription { get; set; }
        public DVariable<string> P1Description { get; set; }
        public DVariable<string> Status { get; set; }
        public DVariable<string> Stage { get; set; }
        public bool IsProjectComplete { get; set; }
        public bool IsProjectRemoved { get; set; }


        /* Priority */
        public DVariable<string> Priority { get; set; }
        public DVariable<string> Probability { get; set; }
        public DVariable<string> PriorityNote { get; set; }
        public DVariable<string> ProbabilityNote { get; set; }
        public DVariable<DateTime> PPLastStatusDate { get; set; }

        /* Project Manager */

        public DVariable<string> PmPriority { get; set; }
        public DVariable<string> PmProbability { get; set; }
        public DVariable<string> PmPriorityNote { get; set; }
        public DVariable<string> PmProbabilityNote { get; set; }
        public DVariable<DateTime> PmPPLastStatusDate { get; set; }


        /* Orgs */
        public DVariable<Guid>? PrimaryOrgId { get; set; }
        public List<Guid>? ParticipatingOrgs { get; set; }


        /* Dates */
        public DVariable<DateTime> H2LPredictedStart { get; set; }
        public DVariable<DateTime> H2LStart { get; set; }
        public DVariable<DateTime> LOPredictedStart { get; set; }
        public DVariable<DateTime> LOStart { get; set; }
        public DVariable<DateTime> SPPredictedStart { get; set; }
        public DVariable<DateTime> SPStart { get; set; }
        public DVariable<DateTime> INDPredictedStart { get; set; }
        public DVariable<DateTime> INDStart { get; set; }
        public DVariable<DateTime> P1PredictedStart { get; set; }
        public DVariable<DateTime> P1Start { get; set; }
        public DateTime? ProjectStatusDate { get; set; }
        public DVariable<DateTime>? CompletionDate { get; set; }
        public DVariable<DateTime>? ProjectRemovedDate { get; set; }

    }
}