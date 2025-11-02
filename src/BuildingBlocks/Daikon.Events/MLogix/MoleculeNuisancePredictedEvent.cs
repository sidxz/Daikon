using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.EventStore.Event;
using Daikon.Shared.Embedded.MLogix;

namespace Daikon.Events.MLogix
{
    public class MoleculeNuisancePredictedEvent : BaseEvent
    {
        public MoleculeNuisancePredictedEvent() : base(nameof(MoleculeNuisancePredictedEvent))
        {

        }
        public Guid MoleculeId { get; set; }
        public string RequestedModelName { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public Nuisance NuisancePrediction { get; set; } = new Nuisance();

    }
}