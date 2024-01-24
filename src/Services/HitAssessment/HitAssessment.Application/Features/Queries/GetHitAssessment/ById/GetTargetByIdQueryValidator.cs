
using FluentValidation;

namespace HitAssessment.Application.Features.Queries.GetHitAssessment.ById
{
    public class GetHitAssessmentByIdQueryValidator : AbstractValidator<GetHitAssessmentByIdQuery>
    {
        public GetHitAssessmentByIdQueryValidator()
        {
            RuleFor(g => g.Id)
            .NotEmpty().WithMessage("{Id} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{Id} is required");
        }
 
    }
}