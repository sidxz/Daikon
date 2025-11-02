using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daikon.Shared.Embedded.MLogix
{
    public class Nuisance
    {
        public Guid Id { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public DateTime PredictionDate { get; set; } = DateTime.UtcNow;
        public int LabelAggregator { get; set; } = default;
        public double ScoreAggregator { get; set; } = 0.0;

        public int LabelLuciferaseInhibitor { get; set; } = default;
        public double ScoreLuciferaseInhibitor { get; set; } = 0.0;

        public int LabelReactive { get; set; } = default;
        public double ScoreReactive { get; set; } = 0.0;

        public int LabelPromiscuous { get; set; } = default;
        public double ScorePromiscuous { get; set; } = 0.0;

        public bool IsVerified { get; set; } = false;
        public string VerifiedLabel { get; set; } = string.Empty;
        public Guid VerifiedBy { get; set; }
        public DateTime VerifiedDate { get; set; }
        public string VerificationNotes { get; set; } = string.Empty;

    }
}