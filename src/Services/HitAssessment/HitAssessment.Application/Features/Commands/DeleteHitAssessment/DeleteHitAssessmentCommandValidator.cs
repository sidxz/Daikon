
using FluentValidation;

namespace HitAssessment.Application.Features.Commands.DeleteHitAssessment
{
    public class DeleteHitAssessmentCommandValidator : AbstractValidator<DeleteHitAssessmentCommand>
    {
        public DeleteHitAssessmentCommandValidator()
        {
            RuleFor(t => t.StrainId)
                .NotEmpty().WithMessage("{StrainId} is required")
                .NotNull();

            
        }
    }
    
}