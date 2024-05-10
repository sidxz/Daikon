
using FluentValidation;

namespace UserStore.Application.Features.Commands.Users.UpdateUser
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {

        public UpdateUserValidator()
        {
            RuleFor(command => command)
                .Must(command => !(string.IsNullOrEmpty(command.Email) && string.IsNullOrEmpty(command.OIDCSub) && string.IsNullOrEmpty(command.EntraObjectId)))
                .WithMessage("Either Email or OIDCSub/EntraObjectId must be provided.");

            When(command => !string.IsNullOrEmpty(command.Email), () =>
            {
                RuleFor(command => command.Email)
                    .Must(email => email.Contains("@"))
                    .WithMessage("Email must be a valid email address.");
            });

            RuleFor(command => command.FirstName)
                .NotEmpty().WithMessage("FirstName is required.")
                .MaximumLength(500).WithMessage("FirstName must not exceed 500 characters.");

            RuleFor(command => command.LastName)
                .NotEmpty().WithMessage("LastName is required.")
                .MaximumLength(500).WithMessage("LastName must not exceed 500 characters.");
        }
    }
}