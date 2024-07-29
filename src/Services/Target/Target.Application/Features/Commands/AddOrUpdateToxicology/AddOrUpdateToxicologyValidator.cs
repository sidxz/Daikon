using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Target.Application.Features.Commands.AddOrUpdateToxicology
{
    public class AddOrUpdateToxicologyValidator : AbstractValidator<AddOrUpdateToxicologyCommand>
    {
        public AddOrUpdateToxicologyValidator()
        {
            RuleFor(e => e.Topic)
                .NotEmpty().WithMessage("{Topic} is required")
                .NotNull();
        }

    }
}