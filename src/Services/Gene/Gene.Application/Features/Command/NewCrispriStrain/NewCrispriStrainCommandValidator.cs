
using FluentValidation;
namespace Gene.Application.Features.Command.NewCrispriStrain
{
    public class NewCrispriStrainCommandValidator : AbstractValidator<NewCrispriStrainCommand>
    {
        public NewCrispriStrainCommandValidator()
        {
            RuleFor(e => e.CrispriStrainName)
            .NotEmpty().WithMessage("{CrispriStrainName} is required")
            .NotNull();

            RuleFor(e => e.GeneId)
            .NotEmpty().WithMessage("{GeneId} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{GeneId} is required");
        }
    }
}