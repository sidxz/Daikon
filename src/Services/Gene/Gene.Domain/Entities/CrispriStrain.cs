
using CQRS.Core.Domain;

namespace Gene.Domain.Entities
{
    public class CrispriStrain : BaseEntity
    {

        public Guid GeneId { get; set; }
        public Guid CrispriStrainId { get; set; }

        public required DVariable<string> CrispriStrainName { get; set; }
        public DVariable<string>? Notes { get; set; }


    }
}