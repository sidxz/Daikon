
using FluentValidation;

namespace Gene.Application.Features.Command.UpdateProteinActivityAssay
{
    public class UpdateProteinActivityAssayCommandValidator : AbstractValidator<UpdateProteinActivityAssayCommand>
    {
        public UpdateProteinActivityAssayCommandValidator()
        {
             RuleFor(e => e.Assay)
            .NotEmpty().WithMessage("{Assay} is required")
            .NotNull();
        }
    }
}