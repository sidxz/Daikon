
using FluentValidation;

namespace MLogix.Application.Features.Queries.GetMolecule.ById
{
    public class GetMoleculeByIdValidator : AbstractValidator<GetMoleculeByIdQuery>
    {
        public GetMoleculeByIdValidator()
        {
            RuleFor(g => g.Id)
            .NotEmpty().WithMessage("{Id} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{Id} is required");
        }
 
    }
}