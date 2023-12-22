
using FluentValidation;

namespace Screen.Application.Features.Commands.UpdateScreen
{
    public class UpdateScreenCommandValidator : AbstractValidator<UpdateScreenCommand>
    {
        public UpdateScreenCommandValidator()
        {
            RuleFor(t => t.StrainId)
            .NotEmpty().WithMessage("{StrainId} is required");
        }
    }
}