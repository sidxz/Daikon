
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

            RuleFor(s => s.Id)
            .NotEmpty().WithMessage("{Id} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{Id} is required");
        }
    }
}