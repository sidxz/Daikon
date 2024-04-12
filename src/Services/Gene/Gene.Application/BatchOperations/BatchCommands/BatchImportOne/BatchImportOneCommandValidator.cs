
using FluentValidation;

namespace Gene.Application.BatchOperations.BatchCommands.BatchImportOne
{
    public class BatchImportOneCommandValidator : AbstractValidator<BatchImportOneCommand>
    {
        public BatchImportOneCommandValidator()
        {
            RuleFor(p => p.AccessionNumber)
            .NotEmpty().WithMessage("{AccessionNumber} is required")
            .NotNull();

            RuleFor(p => p.Source)
            .NotEmpty().WithMessage("{Source} is required")
            .NotNull().WithMessage("{Source} is required");
        }
    }
}