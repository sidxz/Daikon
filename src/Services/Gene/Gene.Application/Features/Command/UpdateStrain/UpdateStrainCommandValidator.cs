
using FluentValidation;

namespace Gene.Application.Features.Command.UpdateStrain
{
    public class UpdateStrainCommandValidator : AbstractValidator<UpdateStrainCommand>
    {
        public UpdateStrainCommandValidator()
        {
            RuleFor(s => s.Name)
            .NotEmpty().WithMessage("{Name} is required")
            .NotNull();
        }
    }
}