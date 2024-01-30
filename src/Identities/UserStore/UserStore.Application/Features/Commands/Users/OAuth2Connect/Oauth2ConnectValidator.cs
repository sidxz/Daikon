
using FluentValidation;

namespace UserStore.Application.Features.Commands.Users.OAuth2Connect
{
    public class OAuth2ConnectValidator : AbstractValidator<OAuth2ConnectCommand>
    {

        public OAuth2ConnectValidator()
        {

            RuleFor(command => command)
                 .Must(command => !(string.IsNullOrEmpty(command.OIDCSub) && string.IsNullOrEmpty(command.EntraObjectId)))
                 .WithMessage("Either OIDCSub or EntraObjectId must be provided.");

            RuleFor(command => command.Email)
                .Must(email => string.IsNullOrEmpty(email) || email.Contains("@"))
                .WithMessage("Email must be a valid email address.");

        }
    }

}