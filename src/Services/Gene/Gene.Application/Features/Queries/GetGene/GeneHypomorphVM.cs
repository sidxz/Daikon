
using CQRS.Core.Domain;

namespace Gene.Application.Features.Queries.GetGene
{
    public class GeneHypomorphVM : DocMetadata
    {
        public Guid Id { get; set; }
        public object KnockdownStrain { get; set; }
        public object Phenotype { get; set; }
        public object GrowthDefect { get; set; }
        public object GrowthDefectSeverity { get; set; }
        public object KnockdownLevel { get; set; }
        public object EstimatedKnockdownRelativeToWT { get; set; }
        public object EstimateBasedOn { get; set; }
        public object SelectivelySensitizesToOnTargetInhibitors { get; set; }
        public object SuitableForScreening { get; set; }
        public object Researcher { get; set; }
        public object Reference { get; set; }
        public object URL { get; set; }
        public object Notes { get; set; }

    }
}