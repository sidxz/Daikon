
using FluentValidation;

namespace MLogix.Application.Features.Queries.GetMolecule.ByRegistrationId
{
    public class GetMoleculeByRegIdValidator : AbstractValidator<GetMoleculeByRegIdQuery>
    {
        public GetMoleculeByRegIdValidator()
        {
            RuleFor(g => g.RegistrationId)
            .NotEmpty().WithMessage("{RegistrationId} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{RegistrationId} is required");
        }
 
    }
}