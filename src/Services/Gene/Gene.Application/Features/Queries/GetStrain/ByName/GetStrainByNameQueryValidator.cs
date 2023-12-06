
using FluentValidation;

namespace Gene.Application.Features.Queries.GetStrain.ByName
{
    public class GetStrainByNameQueryValidator : AbstractValidator<GetStrainByNameQuery>
    {
        public GetStrainByNameQueryValidator()
        {
            RuleFor(s => s.Name)
            .NotEmpty().WithMessage("{Name} is required")
            .NotNull();
        }
 
    }
}