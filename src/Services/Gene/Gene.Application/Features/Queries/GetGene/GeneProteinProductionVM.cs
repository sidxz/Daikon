
using CQRS.Core.Domain;

namespace Gene.Application.Features.Queries.GetGene
{
    public class GeneProteinProductionVM : VMMeta
    {

        public Guid Id { get; set; }
        public object Production { get; set; }
        public object Method { get; set; }
        public object Purity { get; set; }
        public object DateProduced { get; set; }
        public object PMID { get; set; }
        public object Notes { get; set; }
        public object URL { get; set; }

    }
}