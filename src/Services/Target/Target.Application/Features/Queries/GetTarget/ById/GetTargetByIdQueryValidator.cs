
using FluentValidation;

namespace Target.Application.Features.Queries.GetTarget.ById
{
    public class GetTargetByIdQueryValidator : AbstractValidator<GetTargetByIdQuery>
    {
        public GetTargetByIdQueryValidator()
        {
            RuleFor(g => g.Id)
            .NotEmpty().WithMessage("{Id} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{Id} is required");
        }
 
    }
}