
using CQRS.Core.Domain;

namespace Project.Application.Features.Queries.GetProjectList
{
    public class ProjectListVM : DocMetadata
    {
        public Guid Id { get; set; }
        public Guid StrainId { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string ProjectType { get; set; }
        public string LegacyId { get; set; }
        public string Status { get; set; }
        public bool IsProjectComplete { get; set; }
        public bool IsProjectRemoved { get; set; }
        public string Stage { get; set; }

        /* Orgs */
        public Guid PrimaryOrgId { get; set; }

        /* Dates */
        public DateTime H2LPredictedStart { get; set; }
        public DateTime H2LStart { get; set; }
        public DateTime LOPredictedStart { get; set; }
        public DateTime LOStart { get; set; }
        public DateTime SPPredictedStart { get; set; }
        public DateTime SPStart { get; set; }
        public DateTime INDPredictedStart { get; set; }
        public DateTime INDStart { get; set; }
        public DateTime P1PredictedStart { get; set; }
        public DateTime P1Start { get; set; }
        public DateTime ProjectStatusDate { get; set; }
        public DateTime TerminationDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public DateTime ProjectRemovedDate { get; set; }


        public string CompoundEvoLatestSMILES { get; set; }
    }
}