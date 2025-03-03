
using CQRS.Core.Domain;

namespace Gene.Application.Features.Queries.GetGene
{
    public class ExpansionPropVM : VMMeta
    {
        public Guid Id { get; set; }
        public required string ExpansionType { get; set; }
        public object ExpansionValue { get; set; }
    }
}