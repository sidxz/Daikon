
using FluentValidation;
namespace Gene.Application.Features.Command.NewEssentiality
{
    public class NewEssentialityCommandValidator : AbstractValidator<NewEssentialityCommand>
    {
        public NewEssentialityCommandValidator()
        {
            RuleFor(p => p.Classification)
            .NotEmpty().WithMessage("{Classification} is required")
            .NotNull();
        }
    }
}