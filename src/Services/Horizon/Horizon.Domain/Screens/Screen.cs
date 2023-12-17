
using CQRS.Core.Domain;

namespace Horizon.Domain.Screens
{
    public class Screen :BaseEntity
    {
        public string StrainId { get; set; }
        public string ScreenId { get; set; }
        public required string Name { get; set; }
        public List<string> AssociatedTargetsId { get; set; }
        public string ScreenType { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public string PrimaryOrgName { get; set; }
    }
}