using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace DocuStore.Application.Features.Commands.UpdateParsedDoc
{
    public class UpdateParsedDocValidator : AbstractValidator<UpdateParsedDocCommand>
    {
        public UpdateParsedDocValidator()
        {
            RuleFor(p => p.FilePath)
                .NotEmpty().WithMessage("FilePath is required.");
        }

    }
}