
using FluentValidation;

namespace UserStore.Application.Features.Commands.Users.OAuth2Connect
{
    public class Oauth2ConnectValidator : AbstractValidator<Oauth2ConnectCommand>
    {

        public Oauth2ConnectValidator()
        {

            RuleFor(command => command)
                 .Must(command => !(string.IsNullOrEmpty(command.OIDCSub) && command.EntraObjectId == Guid.Empty))
                 .WithMessage("Either OIDCSub or EntraObjectId must be provided.");

            RuleFor(command => command.Email)
                .Must(email => string.IsNullOrEmpty(email) || email.Contains("@"))
                .WithMessage("Email must be a valid email address.");

        }
    }

}