
using FluentValidation;

namespace Screen.Application.Features.Commands.RenameScreen
{
    public class RenameScreenCommandValidator : AbstractValidator<RenameScreenCommand>
    {
        public RenameScreenCommandValidator()
        {
            RuleFor(t => t.Name)
            .NotEmpty().WithMessage("{Name} is required")
            .NotNull();
        }
    }
}