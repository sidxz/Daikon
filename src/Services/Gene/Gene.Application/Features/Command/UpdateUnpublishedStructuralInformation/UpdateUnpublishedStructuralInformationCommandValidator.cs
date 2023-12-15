
using FluentValidation;

namespace Gene.Application.Features.Command.UpdateUnpublishedStructuralInformation
{
    public class UpdateUnpublishedStructuralInformationCommandValidator : AbstractValidator<UpdateUnpublishedStructuralInformationCommand>
    {
        public UpdateUnpublishedStructuralInformationCommandValidator()
        {
             RuleFor(e => e.Organization)
            .NotEmpty().WithMessage("{Organization} is required")
            .NotNull();

            RuleFor(e => e.UnpublishedStructuralInformationId)
            .NotEmpty().WithMessage("{UnpublishedStructuralInformationId} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{UnpublishedStructuralInformationId} is required");
        }
    }
}