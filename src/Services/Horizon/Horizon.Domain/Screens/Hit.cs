
using CQRS.Core.Domain;

namespace Horizon.Domain.Screens
{
    public class Hit : BaseEntity
    {
        public string Library { get; set; }
        public string StrainId { get; set; }
        public string HitCollectionId { get; set; }
        public string HitId { get; set; }
        public string InitialStructureSMILES { get; set; }
        public string CompoundId { get; set; }
    }
}