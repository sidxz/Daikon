
using FluentValidation;

namespace Screen.Application.Features.Commands.NewHit
{
    public class NewHitCommandValidator : AbstractValidator<NewHitCommand>
    {
        public NewHitCommandValidator()
        {
            RuleFor(t => t.Id)
                .NotEmpty().WithMessage("{Id} is required")
                .NotNull();

        }

    }
}