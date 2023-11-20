using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Gene.Application.Features.Command.UpdateGene
{
    public class UpdateGeneCommandValidator : AbstractValidator<UpdateGeneCommand>
    {
        public UpdateGeneCommandValidator()
        {
            RuleFor(p => p.AccessionNumber)
            .NotEmpty().WithMessage("{AccessionNumber} is required")
            .NotNull();
        }
    }
}