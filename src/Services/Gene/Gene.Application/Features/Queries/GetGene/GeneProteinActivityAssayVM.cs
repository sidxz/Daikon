
using CQRS.Core.Domain;

namespace Gene.Application.Features.Queries.GetGene
{
    public class GeneProteinActivityAssayVM : VMMeta
    {
        public Guid Id { get; set; }
        public object Assay { get; set; }
        public object Method { get; set; }
        public object Throughput { get; set; }
        public object PMID { get; set; }
        public object Reference { get; set; }
        public object URL { get; set; }

    }
}