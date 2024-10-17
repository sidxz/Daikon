
using CQRS.Core.Domain;

namespace Gene.Application.Features.Queries.GetGene
{
    public class GeneResistanceMutationVM : VMMeta
    {
        public Guid Id { get; set; }
        public object Mutation { get; set; }
        public object Isolate { get; set; }
        public object ParentStrain { get; set; }
        public object Compound { get; set; }
        public object ShiftInMIC { get; set; }
        public object Organization { get; set; }
        public object Researcher { get; set; }
        public object Reference { get; set; }
        public object Notes { get; set; }
        

    }
}