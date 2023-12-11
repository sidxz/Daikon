
using FluentValidation;
namespace Gene.Application.Features.Command.NewEssentiality
{
    public class NewEssentialityCommandValidator : AbstractValidator<NewEssentialityCommand>
    {
        public NewEssentialityCommandValidator()
        {
            RuleFor(e => e.Classification)
            .NotEmpty().WithMessage("{Classification} is required")
            .NotNull();

            RuleFor(e => e.GeneId)
            .NotEmpty().WithMessage("{GeneId} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{GeneId} is required");
        }
    }
}