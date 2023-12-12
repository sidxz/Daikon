
using FluentValidation;

namespace Gene.Application.Features.Command.UpdateHypomorph
{
    public class UpdateHypomorphCommandValidator : AbstractValidator<UpdateHypomorphCommand>
    {
        public UpdateHypomorphCommandValidator()
        {
             RuleFor(e => e.KnockdownStrain)
            .NotEmpty().WithMessage("{KnockdownStrain} is required")
            .NotNull();

            RuleFor(e => e.HypomorphId)
            .NotEmpty().WithMessage("{HypomorphId} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{HypomorphId} is required");
        }
    }
}