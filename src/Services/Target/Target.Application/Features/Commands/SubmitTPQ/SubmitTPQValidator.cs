using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Target.Application.Features.Commands.SubmitTPQ
{
    public class SubmitTPQValidator : AbstractValidator<SubmitTPQCommand>
    {
        public SubmitTPQValidator()
        {
            RuleFor(p => p.RequestedTargetName)
                .NotEmpty().WithMessage("Target Name is required.")
                .NotNull()
                .MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters.");
            RuleFor(p => p.RequestedAssociatedGenes)
                .NotEmpty().WithMessage("Associated Gene(s) is required.")
                .NotNull();
            RuleFor(p => p.Response)
                .NotEmpty().WithMessage("Answers are required.")
                .NotNull();
        }
    }
}