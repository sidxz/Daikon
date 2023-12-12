
namespace Gene.Application.Features.Queries.GetGene
{
    public class GeneHypomorphVM
    {
        public Guid HypomorphId { get; set; }

        public object KnockdownStrain { get; set; }
        public object Phenotype { get; set; }
        public object Notes { get; set; }

    }
}