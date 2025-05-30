using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace MLogix.Application.Features.Commands.RegisterUndisclosed
{
    public class RegisterUndisclosedValidator : AbstractValidator<RegisterUndisclosedCommand>
    {
        public RegisterUndisclosedValidator()
        {

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Name is required.")
                .NotNull();
        }

    }
}