
using CQRS.Core.Domain;

namespace Horizon.Domain.MLogix
{
    public class Molecule : BaseEntity
    {
        public string Name { get; set; }
        public string RegistrationId { get; set; }
        public string MLogixId { get; set; }
        public string RequestedSMILES { get; set; }
        public string SmilesCanonical { get; set; }
    }
}