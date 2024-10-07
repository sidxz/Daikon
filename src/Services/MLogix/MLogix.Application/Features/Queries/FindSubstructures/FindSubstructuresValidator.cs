
using FluentValidation;

namespace MLogix.Application.Features.Queries.FindSubstructures
{
    public class FindSubstructuresValidator : AbstractValidator<FindSubstructuresQuery>
    {
        public FindSubstructuresValidator()
        {
            RuleFor(g => g.SMILES)
            .NotEmpty().WithMessage("{SMILES} is required")
            .NotNull();
        }

    }
}