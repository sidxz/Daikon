
using FluentValidation;

namespace Screen.Application.Features.Commands.DeleteScreen
{
    public class DeleteScreenCommandValidator : AbstractValidator<DeleteScreenCommand>
    {
        public DeleteScreenCommandValidator()
        {
            RuleFor(t => t.StrainId)
                .NotEmpty().WithMessage("{StrainId} is required")
                .NotNull();

            
        }
    }
    
}