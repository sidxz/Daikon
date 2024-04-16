
using CQRS.Core.Domain;
using Project.Application.Features.Queries.GetProject;

namespace Project.Application.Features.Queries.GetProject
{
    public class ProjectVM : DocMetadata
    {
        public Guid Id { get; set; }
        public Guid StrainId { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string ProjectType { get; set; }
        public string LegacyId { get; set; }
        public object Description { get; set; }
        public object Status { get; set; }
        public bool IsProjectComplete { get; set; }
        public bool IsProjectRemoved { get; set; }
        public object Stage { get; set; }

        /* Associated Hit Assessment */
        public Guid HaId { get; set; }
        public Guid CompoundId { get; set; }
        public string CompoundSMILES { get; set; }
        public Guid HitCompoundId { get; set; }
        public Guid HitId { get; set; }


        /* Priority */
        public object Priority { get; set; }
        public object Probability { get; set; }
        public object PriorityNote { get; set; }
        public object ProbabilityNote { get; set; }
        public object PPLastStatusDate { get; set; }

        /* Project Manager */

        public object PmPriority { get; set; }
        public object PmProbability { get; set; }
        public object PmPriorityNote { get; set; }
        public object PmProbabilityNote { get; set; }
        public object PmPPLastStatusDate { get; set; }


        /* Orgs */
        public object PrimaryOrgId { get; set; }
        public List<Guid> ParticipatingOrgs { get; set; }


        /* Dates */
        public object H2LPredictedStart { get; set; }
        public object H2LStart { get; set; }
        public object LOPredictedStart { get; set; }
        public object LOStart { get; set; }
        public object SPPredictedStart { get; set; }
        public object SPStart { get; set; }
        public object INDPredictedStart { get; set; }
        public object INDStart { get; set; }
        public object P1PredictedStart { get; set; }
        public object P1Start { get; set; }
        public DateTime ProjectStatusDate { get; set; }
        public DateTime TerminationDate { get; set; }
        public object CompletionDate { get; set; }
        public object ProjectRemovedDate { get; set; }

        /* Compound Evolution */
        public List<CompoundEvolutionVM> CompoundEvolution { get; set; }
        public Guid CompoundEvoLatestMoleculeId { get; set; }
        public string CompoundEvoLatestSMILES { get; set; } // Calculated

    }
}