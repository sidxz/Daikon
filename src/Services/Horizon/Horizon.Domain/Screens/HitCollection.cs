
using CQRS.Core.Domain;

namespace Horizon.Domain.Screens
{
    public class HitCollection : BaseEntity
    {
        public string StrainId { get; set; }
        public string ScreenId { get; set; }
        public string HitCollectionId { get; set; }
        public string Name { get; set; }
        public string HitCollectionType { get; set; }
        public string PrimaryOrgName { get; set; } 
    }
}