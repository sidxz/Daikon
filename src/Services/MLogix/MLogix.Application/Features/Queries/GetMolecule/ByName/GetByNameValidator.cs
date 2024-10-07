
using FluentValidation;

namespace MLogix.Application.Features.Queries.GetMolecule.ByName
{
    public class GetByNameValidator : AbstractValidator<GetByNameQuery>
    {
        public GetByNameValidator()
        {
            RuleFor(g => g.Name)
            .NotEmpty().WithMessage("{NAME} is required")
            .NotNull();
        }

    }
}