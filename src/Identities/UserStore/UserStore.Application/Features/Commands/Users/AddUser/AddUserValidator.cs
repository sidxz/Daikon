
using FluentValidation;

namespace UserStore.Application.Features.Commands.Users.AddUser
{
    public class AddUserValidator : AbstractValidator<AddUserCommand>
    {

        public AddUserValidator()
        {
            
            RuleFor(command => command)
                 .Must(command => !(string.IsNullOrEmpty(command.Email) && string.IsNullOrEmpty(command.OIDCSub)))
                 .WithMessage("Either Email or OIDCSub must be provided.");
            
            RuleFor(command => command.Email)
                .Must(email => string.IsNullOrEmpty(email) || email.Contains("@"))
                .WithMessage("Email must be a valid email address.");

            RuleFor(command => command.FirstName)
                .NotEmpty().WithMessage("FirstName is required.")
                .MaximumLength(50).WithMessage("FirstName must not exceed 50 characters.");

            RuleFor(command => command.LastName)
                .NotEmpty().WithMessage("LastName is required.")
                .MaximumLength(50).WithMessage("LastName must not exceed 50 characters.");

        }
    }
}