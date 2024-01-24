
using FluentValidation;

namespace Screen.Application.Features.Queries.GetScreen.ByName
{
    public class GetScreenByNameQueryValidator : AbstractValidator<GetScreenByNameQuery>
    {
        public GetScreenByNameQueryValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
    }
}