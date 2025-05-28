
using CQRS.Core.Domain;

namespace Gene.Application.Features.Queries.GetGene
{
    public class GeneUnpublishedStructuralInformationVM : VMMeta
    {
        public Guid Id { get; set; }
        public object Organization { get; set; }
        public object Method { get; set; }
        public object Resolution { get; set; }
        public object ResolutionUnit { get; set; }
        public object Ligands { get; set; }
        public object Researcher { get; set; }

        public object Reference { get; set; }
        public object Notes { get; set; }
        public object URL { get; set; }
        

    }
}