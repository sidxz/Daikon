
using FluentValidation;

namespace Gene.Application.Features.Command.UpdateEssentiality
{
    public class UpdateEssentialityCommandValidator : AbstractValidator<UpdateEssentialityCommand>
    {
        public UpdateEssentialityCommandValidator()
        {
             RuleFor(e => e.Classification)
            .NotEmpty().WithMessage("{Classification} is required")
            .NotNull();

            RuleFor(e => e.EssentialityId)
            .NotEmpty().WithMessage("{EssentialityId} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{EssentialityId} is required");
        }
    }
}