

using CQRS.Core.Domain.Historical;

namespace Target.Domain.EntityRevisions
{
    public class TargetRevision : BaseVersionEntity
    {
        public DVariableHistory<string> Bucket { get; set; }

        public DVariableHistory<double> ImpactScore { get; set; }

        public DVariableHistory<double> ImpactComplete { get; set; }

        public DVariableHistory<double> LikeScore { get; set; }

        public DVariableHistory<double> LikeComplete { get; set; }

        public DVariableHistory<double> ScreeningScore { get; set; }

        public DVariableHistory<double> ScreeningComplete { get; set; }

        public DVariableHistory<double> StructureScore { get; set; }

        public DVariableHistory<double> StructureComplete { get; set; }

        public DVariableHistory<double> VulnerabilityRatio { get; set; }

        public DVariableHistory<double> VulnerabilityRank { get; set; }

        public DVariableHistory<double> HTSFeasibility { get; set; }

        public DVariableHistory<double> SBDFeasibility { get; set; }

        public DVariableHistory<double> Progressibility { get; set; }

        public DVariableHistory<double> Safety { get; set; }
    }
}