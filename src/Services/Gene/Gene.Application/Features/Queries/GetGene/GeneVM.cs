
namespace Gene.Application.Features.Queries.GetGene
{
    public class GeneVM
    {
        public Guid Id { get; set; }
        public Guid StrainId { get; set; }
        public string AccessionNumber { get; set; }
        public string Name { get; set; }
        public object Function { get; set; }
        public object Product { get; set; }
        public object FunctionalCategory { get; set; }

        public List<GeneEssentialityVM> Essentialities { get; set; }
        public List<GeneProteinProductionVM> ProteinProductions { get; set; }
        public List<GeneProteinActivityAssayVM> ProteinActivityAssays { get; set; }
        public List<GeneHypomorphVM> Hypomorphs { get; set; }
        public List<GeneCrispriStrainVM> CrispriStrains { get; set; }

    }
}