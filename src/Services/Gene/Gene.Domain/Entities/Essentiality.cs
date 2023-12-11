
using CQRS.Core.Domain;

namespace Gene.Domain.Entities
{
    public class Essentiality : BaseEntity
    {
        
        public Guid EssentialityId { get; set; }
        public required DVariable<string> Classification { get; set; }
        public DVariable<string>? Condition { get; set; }
        public DVariable<string>? Method { get; set; }
        public DVariable<string>? Reference { get; set; }
        public DVariable<string>? Note { get; set; }
    }
}