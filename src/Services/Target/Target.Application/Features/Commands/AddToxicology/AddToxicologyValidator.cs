using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Target.Application.Features.Commands.AddToxicology
{
    public class AddToxicologyValidator : AbstractValidator<AddToxicologyCommand>
    {
        public AddToxicologyValidator()
        {
            RuleFor(e => e.Topic)
                .NotEmpty().WithMessage("{Topic} is required")
                .NotNull();
        }

    }
}