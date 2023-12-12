
using CQRS.Core.Domain;

namespace Gene.Domain.Entities
{
    public class Hypomorph : BaseEntity
    {

        public Guid GeneId { get; set; }
        public Guid HypomorphId { get; set; }

        public required DVariable<string> KnockdownStrain { get; set; }
        public DVariable<string>? Phenotype { get; set; }
        public DVariable<string>? Notes { get; set; }


    }
}