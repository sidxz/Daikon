
using CQRS.Core.Domain;

namespace Gene.Domain.Entities
{
    public class Gene : BaseEntity
    {
        
        
        public Guid StrainId { get; set; }
        public string AccessionNumber { get; set; }
        public string Name { get; set; }
        public DVariable<string> Function { get; set; }
        public DVariable<string> Product { get; set; }
        public DVariable<string> FunctionalCategory { get; set; }
        

        
    }
}