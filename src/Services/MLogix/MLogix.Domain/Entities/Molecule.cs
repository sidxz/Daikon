
using CQRS.Core.Domain;

namespace MLogix.Domain.Entities
{
    public class Molecule : BaseEntity
    {
        public Guid RegistrationId { get; set; }
        public string RegistrationStatus { get; set; }
        public bool IsStructureDisclosed { get; set; }
        public string? RequestedSMILES { get; set; }
        public DVariable<string> Name { get; set; }
        public Dictionary<string, string>? Ids { get; set; }
        
    }
}