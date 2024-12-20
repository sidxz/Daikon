
using CQRS.Core.Domain;

namespace Gene.Application.Features.Queries.GetGene
{
    public class GeneEssentialityVM : VMMeta
    {
        public Guid Id { get; set; }
        public object Classification { get; set; }
        public object Condition { get; set; }
        public object Method { get; set; }
        public object Reference { get; set; }
        public object Note { get; set; }

    }
}