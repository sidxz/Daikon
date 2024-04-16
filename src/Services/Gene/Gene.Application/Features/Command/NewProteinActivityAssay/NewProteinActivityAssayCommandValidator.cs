
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

        }
    }
}