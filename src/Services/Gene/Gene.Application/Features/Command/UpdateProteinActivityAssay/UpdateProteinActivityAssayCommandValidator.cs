
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

            RuleFor(e => e.ProteinActivityAssayId)
            .NotEmpty().WithMessage("{ProteinActivityAssayId} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{ProteinActivityAssayId} is required");
        }
    }
}