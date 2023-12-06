
using FluentValidation;
namespace Gene.Application.Features.Command.NewStrain
{
    public class NewStrainCommandValidator : AbstractValidator<NewStrainCommand>
    {
        public NewStrainCommandValidator()
        {
            RuleFor(s => s.Name)
            .NotEmpty().WithMessage("{StrainName} is required")
            .NotNull();
        }
    }
}