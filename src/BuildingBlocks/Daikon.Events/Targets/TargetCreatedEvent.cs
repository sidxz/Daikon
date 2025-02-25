
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Targets
{
    public class TargetCreatedEvent : BaseEvent
    {
        public TargetCreatedEvent() : base(nameof(TargetCreatedEvent))
        {
            
        }

        public Guid StrainId { get; set; }
        public required string Name { get; set; }
        public Dictionary<string, string> AssociatedGenes { get; set; }
        public string TargetType { get; set; }
        public DVariable<string> Bucket { get; set; }

        public DVariable<double> ImpactScore { get; set; }

        public DVariable<double> ImpactComplete { get; set; }

        public DVariable<double> LikeScore { get; set; }

        public DVariable<double> LikeComplete { get; set; }

        public DVariable<double> ScreeningScore { get; set; }

        public DVariable<double> ScreeningComplete { get; set; }

        public DVariable<double> StructureScore { get; set; }

        public DVariable<double> StructureComplete { get; set; }

        public DVariable<double> VulnerabilityRatio { get; set; }

        public DVariable<double> VulnerabilityRank { get; set; }

        public DVariable<double> HTSFeasibility { get; set; }

        public DVariable<double> SBDFeasibility { get; set; }

        public DVariable<double> Progressibility { get; set; }

        public DVariable<double> Safety { get; set; }

        public DVariable<string> Background { get; set; }

        public DVariable<string> Enablement { get; set; }

        public DVariable<string> Strategy { get; set; }

        public DVariable<string> Challenges { get; set; }
        public int Priority { get; set; } = default!;
        
    }
}