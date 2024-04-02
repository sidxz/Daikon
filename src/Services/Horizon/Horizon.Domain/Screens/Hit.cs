

namespace Horizon.Domain.Screens
{
    public class Hit : GraphEntity
    {
        public string Library { get; set; }
        public string StrainId { get; set; }
        public string HitCollectionId { get; set; }
        public string HitId { get; set; }
        public string RequestedSMILES { get; set; }
        public string MoleculeId { get; set; }
        public string MoleculeRegistrationId { get; set; }
    }
}