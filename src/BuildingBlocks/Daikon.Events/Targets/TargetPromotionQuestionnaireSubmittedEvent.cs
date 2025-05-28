using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.EventStore.Event;

namespace Daikon.Events.Targets
{
    public class TargetPromotionQuestionnaireSubmittedEvent : BaseEvent
    {
        public TargetPromotionQuestionnaireSubmittedEvent() : base(nameof(TargetPromotionQuestionnaireSubmittedEvent))
        {

        }
        public Guid RequestedBy { get; set; }
        public required string RequestedTargetName { get; set; }
        public Dictionary<string, string>? RequestedAssociatedGenes { get; set; }
        public Guid StrainId { get; set; }
        public List<Tuple<string, string, string>> Response { get; set; }

    }
}