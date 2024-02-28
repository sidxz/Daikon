
using FluentValidation;

namespace MLogix.Application.Features.Queries.GetMolecule.BySMILES
{
    public class GetMoleculeBySMILESValidator : AbstractValidator<GetMoleculeBySMILESQuery>
    {
        public GetMoleculeBySMILESValidator()
        {
            RuleFor(g => g.SMILES)
            .NotEmpty().WithMessage("{SMILES} is required")
            .NotNull();
        }
 
    }
}