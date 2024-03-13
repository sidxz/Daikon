using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace Questionnaire.Domain
{
    public class Questionnaire : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public List<Question> Questions { get; set; }
    }
}