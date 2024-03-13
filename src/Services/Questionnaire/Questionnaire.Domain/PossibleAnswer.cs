using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace Questionnaire.Domain
{
    public class PossibleAnswer : BaseEntity
    {
        public Guid QuestionId { get; set; }
        public string Answer { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public bool IsInverted { get; set; }
    }
}