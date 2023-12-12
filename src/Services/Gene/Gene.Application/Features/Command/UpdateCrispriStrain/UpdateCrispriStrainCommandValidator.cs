
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

            RuleFor(e => e.CrispriStrainId)
            .NotEmpty().WithMessage("{CrispriStrainId} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{CrispriStrainId} is required");
        }
    }
}