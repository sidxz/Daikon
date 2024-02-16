
using FluentValidation;

namespace UserStore.Application.Features.Commands.Users.ResolvePermission
{
    public class ResolvePermissionValidator : AbstractValidator<ResolvePermissionQuery>
    {

        public ResolvePermissionValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Method).NotEmpty()
            .Must(x => new List<string> { "GET", "POST", "PUT", "DELETE" }.Contains(x))
            .WithMessage("Method must be one of the following: GET, POST, PUT, DELETE");
            RuleFor(x => x.Endpoint).NotEmpty()
            .WithMessage("Endpoint must not be empty");
        }
    }
}