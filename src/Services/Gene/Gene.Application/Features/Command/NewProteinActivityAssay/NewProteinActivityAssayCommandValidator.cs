
using FluentValidation;
namespace Gene.Application.Features.Command.NewProteinActivityAssay
{
    public class NewProteinActivityAssayCommandValidator : AbstractValidator<NewProteinActivityAssayCommand>
    {
        public NewProteinActivityAssayCommandValidator()
        {
            RuleFor(e => e.Assay)
            .NotEmpty().WithMessage("{Assay} is required")
            .NotNull();

            RuleFor(e => e.GeneId)
            .NotEmpty().WithMessage("{GeneId} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{GeneId} is required");
        }
    }
}