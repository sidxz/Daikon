
using CQRS.Core.Domain.Historical;

namespace Project.Domain.EntityRevisions
{
    public class ProjectRevision : BaseVersionEntity
    {

        public DVariableHistory<string> Description { get; set; }
        public DVariableHistory<string> Status { get; set; }
        public DVariableHistory<string> Stage { get; set; }

        /* Orgs */
        public DVariableHistory<Guid> PrimaryOrgId { get; set; }


        /* Priority */
        public DVariableHistory<string> Priority { get; set; }
        public DVariableHistory<string> Probability { get; set; }
        public DVariableHistory<string> PriorityNote { get; set; }
        public DVariableHistory<string> ProbabilityNote { get; set; }
        public DVariableHistory<DateTime> PPLastStatusDate { get; set; }

        /* Project Manager */

        public DVariableHistory<string> PmPriority { get; set; }
        public DVariableHistory<string> PmProbability { get; set; }
        public DVariableHistory<string> PmPriorityNote { get; set; }
        public DVariableHistory<string> PmProbabilityNote { get; set; }
        public DVariableHistory<DateTime> PmPPLastStatusDate { get; set; }
        

        /* Dates */
        public DVariableHistory<DateTime> H2LPredictedStart { get; set; }
        public DVariableHistory<DateTime> H2LStart { get; set; }
        public DVariableHistory<DateTime> LOPredictedStart { get; set; }
        public DVariableHistory<DateTime> LOStart { get; set; }
        public DVariableHistory<DateTime> SPPredictedStart { get; set; }
        public DVariableHistory<DateTime> SPStart { get; set; }
        public DVariableHistory<DateTime> INDPredictedStart { get; set; }
        public DVariableHistory<DateTime> INDStart { get; set; }
        public DVariableHistory<DateTime> P1PredictedStart { get; set; }
        public DVariableHistory<DateTime> P1Start { get; set; }
        public DVariableHistory<DateTime> CompletionDate { get; set; }
        public DVariableHistory<DateTime> ProjectRemovedDate { get; set; }
    }
}