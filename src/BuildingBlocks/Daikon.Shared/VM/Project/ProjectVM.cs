
using CQRS.Core.Domain;

namespace Daikon.Shared.VM.Project
{
    public class ProjectVM : VMMeta
    {
        public Guid Id { get; set; }
        public Guid StrainId { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string ProjectType { get; set; }
        public string LegacyId { get; set; }
        public string Description { get; set; }
        public string H2LDescription { get; set; }
        public string LODescription { get; set; }
        public string SPDescription { get; set; }
        public string INDDescription { get; set; }
        public string P1Description { get; set; }
        public string Status { get; set; }
        public bool IsProjectComplete { get; set; }
        public bool IsProjectRemoved { get; set; }
        public string Stage { get; set; }

        /* Associated Hit Assessment */
        public Guid HaId { get; set; }
        public Guid CompoundId { get; set; }
        public string CompoundSMILES { get; set; }
        public Guid HitCompoundId { get; set; }
        public Guid HitId { get; set; }


        /* Priority */
        public string Priority { get; set; }
        public string Probability { get; set; }
        public string PriorityNote { get; set; }
        public string ProbabilityNote { get; set; }
        public string PPLastStatusDate { get; set; }

        /* Project Manager */

        public string PmPriority { get; set; }
        public string PmProbability { get; set; }
        public string PmPriorityNote { get; set; }
        public string PmProbabilityNote { get; set; }
        public string PmPPLastStatusDate { get; set; }


        /* Orgs */
        public Guid PrimaryOrgId { get; set; }
        public List<Guid> ParticipatingOrgs { get; set; }


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
        public DateTime CompletionDate { get; set; }
        public DateTime ProjectRemovedDate { get; set; }

        /* Compound Evolution */
        public List<CompoundEvolutionVM> CompoundEvolution { get; set; }
        public Guid CompoundEvoLatestMoleculeId { get; set; }
        public string CompoundEvoLatestSMILES { get; set; } // Calculated

    }
}