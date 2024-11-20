
using CQRS.Core.Domain;

namespace Target.Application.Features.Queries.GetTarget
{
    public class TargetVM : VMMeta
    {
        public Guid StrainId { get; set; }
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public Dictionary<string, string> AssociatedGenes { get; set; }
        public string AssociatedGenesFlattened { get; set; }

        public string TargetType { get; set; }
        public object Bucket { get; set; }
        public object ImpactScore { get; set; }
        public object ImpactComplete { get; set; }
        public object LikeScore { get; set; }
        public object LikeComplete { get; set; }
        public object ScreeningScore { get; set; }
        public object ScreeningComplete { get; set; }
        public object StructureScore { get; set; }
        public object StructureComplete { get; set; }
        public object VulnerabilityRatio { get; set; }
        public object VulnerabilityRank { get; set; }
        public object HTSFeasibility { get; set; }
        public object SBDFeasibility { get; set; }
        public object Progressibility { get; set; }
        public object Safety { get; set; }

        public object Background { get; set; }
        public object Enablement { get; set; }
        public object Strategy { get; set; }
        public object Challenges { get; set; }


    }
}