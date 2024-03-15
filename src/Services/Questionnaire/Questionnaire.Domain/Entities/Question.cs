
using CQRS.Core.Domain;

namespace Questionnaire.Domain.Entities
{
    public class Question : BaseEntity
    {
        public string Identification { get; set; }
        public string? Module { get; set; }
        public string? Section { get; set; }
        public string? SubSection { get; set; }
        public string? SectionDescription { get; set; }
        public string? SubSectionDescription { get; set; }
        public string QuestionBody { get; set; }
        public string? Notes { get; set; }
        public string? ToolTip { get; set; }
        public string QuestionType { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsMultiple { get; set; }
        public bool? IsInverted { get; set; }
        public double? Weight { get; set; }
        public bool? IsAdminOnly { get; set; }
        public List<PossibleAnswer> PossibleAnswers { get; set; } = [];
    }
}