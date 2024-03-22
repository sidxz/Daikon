using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Target.Application.Features.Commands.RenameTarget
{
    public class RenameTargetValidator : AbstractValidator<RenameTargetCommand>
    {
        public RenameTargetValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
    }
}