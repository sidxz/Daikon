
using CQRS.Core.Domain;

namespace Gene.Domain.Entities
{
    public class Hypomorph : BaseEntity
    {

        public Guid GeneId { get; set; }
        public Guid HypomorphId { get; set; }
        public DVariable<string> KnockdownStrain { get; set; }
        public DVariable<string> Phenotype { get; set; }
        public DVariable<string> GrowthDefect { get; set; }
        public DVariable<string> GrowthDefectSeverity { get; set; }
        public DVariable<string> KnockdownLevel { get; set; }
        public DVariable<string> EstimatedKnockdownRelativeToWT { get; set; }
        public DVariable<string> EstimateBasedOn { get; set; }
        public DVariable<string> SelectivelySensitizesToOnTargetInhibitors { get; set; }
        public DVariable<string> SuitableForScreening { get; set; }
        public DVariable<string> Researcher { get; set; }
        public DVariable<string> Reference { get; set; }
        public DVariable<string> URL { get; set; }
        public DVariable<string> Notes { get; set; }


    }
}