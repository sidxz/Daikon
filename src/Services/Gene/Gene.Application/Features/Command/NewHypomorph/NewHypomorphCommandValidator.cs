
using FluentValidation;
namespace Gene.Application.Features.Command.NewHypomorph
{
    public class NewHypomorphCommandValidator : AbstractValidator<NewHypomorphCommand>
    {
        public NewHypomorphCommandValidator()
        {
            // RuleFor(e => e.KnockdownStrain)
            // .NotEmpty().WithMessage("{KnockdownStrain} is required")
            // .NotNull();
        }
    }
}