
using CQRS.Core.Domain;

namespace Questionnaire.Domain.Entities
{
    public class Questionnaire : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Version { get; set; }
        public List<Question> Questions { get; set; } = [];
    }
}