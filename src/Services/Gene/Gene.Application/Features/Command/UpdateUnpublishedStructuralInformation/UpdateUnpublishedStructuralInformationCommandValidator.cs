
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
        }
    }
}