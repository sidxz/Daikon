using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Event;

namespace Daikon.Events.Targets
{
    public class TargetPromotionQuestionnaireUpdatedEvent : BaseEvent
    {
        public TargetPromotionQuestionnaireUpdatedEvent() : base(nameof(TargetPromotionQuestionnaireUpdatedEvent))
        {

        }
        public Guid? TargetId { get; set; }
        public Guid RequestedBy { get; set; }
        public required string RequestedTargetName { get; set; }
        public required string ApprovedTargetName { get; set; }
        public Dictionary<string, string>? RequestedAssociatedGenes { get; set; }
        public Dictionary<string, string>? ApprovedAssociatedGenes { get; set; }
        public Guid StrainId { get; set; }
        public List<Tuple<string, string, string>> Response { get; set; }

    }
}