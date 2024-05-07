
using FluentValidation;

namespace MLogix.Application.Features.Queries.FindSimilarMolecules
{
    public class FindSimilarMoleculesValidator : AbstractValidator<FindSimilarMoleculesQuery>
    {
        public FindSimilarMoleculesValidator()
        {
            RuleFor(g => g.SMILES)
            .NotEmpty().WithMessage("{SMILES} is required")
            .NotNull();
        }
 
    }
}