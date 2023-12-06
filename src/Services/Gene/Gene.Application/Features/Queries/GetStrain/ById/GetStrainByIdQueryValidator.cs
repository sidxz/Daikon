
using FluentValidation;

namespace Gene.Application.Features.Queries.GetStrain.ById
{
    public class GetStrainByIdQueryValidator : AbstractValidator<GetStrainByIdQuery>
    {
        public GetStrainByIdQueryValidator()
        {
            RuleFor(g => g.Id)
            .NotEmpty().WithMessage("{Id} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{Id} is required");
        }
 
    }
}