
using CQRS.Core.Domain;
using Daikon.EventStore.Event;

namespace Daikon.Events.Gene
{
    public class GeneUpdatedEvent : BaseEvent
    {
        public GeneUpdatedEvent() : base(nameof(GeneUpdatedEvent))
        {

        }

        public string UniProtKB { get; set; }
        public string Name { get; set; }
        public string ProteinNameExpanded { get; set; }
        public string AlphaFoldId { get; set; }

        public DVariable<string> Product { get; set; }
        public DVariable<string> FunctionalCategory { get; set; }
        public DVariable<string> Comments { get; set; }

        public Tuple<string, string, string> Coordinates { get; set; } // (start, end, orientation)
        public List<Tuple<string, string>> Orthologues { get; set; } // (type, description)

        public DVariable<string> GeneSequence { get; set; }
        public DVariable<string> ProteinSequence { get; set; }
        public string GeneLength { get; set; }
        public string ProteinLength { get; set; }


    }
}