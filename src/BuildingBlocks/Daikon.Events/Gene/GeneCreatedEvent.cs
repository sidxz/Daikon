
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Gene
{
    public class GeneCreatedEvent : BaseEvent
    {
        public GeneCreatedEvent() : base(nameof(GeneCreatedEvent))
        {

        }


        public Guid StrainId { get; set; }
        public string AccessionNumber { get; set; }
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