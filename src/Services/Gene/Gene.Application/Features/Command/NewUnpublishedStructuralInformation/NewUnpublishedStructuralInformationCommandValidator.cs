
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

            RuleFor(e => e.GeneId)
            .NotEmpty().WithMessage("{GeneId} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{GeneId} is required");
        }
    }
}