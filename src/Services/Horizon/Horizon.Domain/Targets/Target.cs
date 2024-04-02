

namespace Horizon.Domain.Targets
{
    public class Target: GraphEntity
    {
        public string TargetId { get; set; }
        public List<string> GeneAccessionNumbers { get; set; }
        public string Name { get; set; }
        public string StrainId { get; set; }
        public string TargetType { get; set; }
        public string Bucket { get; set; }
    }
}