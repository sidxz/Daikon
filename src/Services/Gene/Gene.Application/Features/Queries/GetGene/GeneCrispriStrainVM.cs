
using CQRS.Core.Domain;

namespace Gene.Application.Features.Queries.GetGene
{
    public class GeneCrispriStrainVM : DocMetadata
    {
        public Guid Id { get; set; }
        public object CrispriStrainName { get; set; }
        public object Notes { get; set; }

    }
}