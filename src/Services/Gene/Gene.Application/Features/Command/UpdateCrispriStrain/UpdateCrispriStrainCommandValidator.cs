
using FluentValidation;

namespace Gene.Application.Features.Command.UpdateCrispriStrain
{
    public class UpdateCrispriStrainCommandValidator : AbstractValidator<UpdateCrispriStrainCommand>
    {
        public UpdateCrispriStrainCommandValidator()
        {
             RuleFor(e => e.CrispriStrainName)
            .NotEmpty().WithMessage("{CrispriStrainName} is required")
            .NotNull();
        }
    }
}