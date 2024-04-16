
using FluentValidation;
namespace Gene.Application.Features.Command.NewUnpublishedStructuralInformation
{
    public class NewUnpublishedStructuralInformationCommandValidator : AbstractValidator<NewUnpublishedStructuralInformationCommand>
    {
        public NewUnpublishedStructuralInformationCommandValidator()
        {
            RuleFor(e => e.Organization)
            .NotEmpty().WithMessage("{Organization} is required")
            .NotNull();
        }
    }
}