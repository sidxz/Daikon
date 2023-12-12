
using FluentValidation;
using Gene.Application.Features.Command.BatchImportOne;
namespace Gene.Application.Features.Command.NewGene
{
    public class BatchImportOneCommandValidator : AbstractValidator<BatchImportOneCommand>
    {
        public BatchImportOneCommandValidator()
        {
            RuleFor(p => p.AccessionNumber)
            .NotEmpty().WithMessage("{AccessionNumber} is required")
            .NotNull();
        }
    }
}