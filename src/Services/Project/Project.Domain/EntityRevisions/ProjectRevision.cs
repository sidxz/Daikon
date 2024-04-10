
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