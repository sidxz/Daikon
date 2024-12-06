using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace DocuStore.Application.Features.Commands.AddParsedDoc
{
    public class AddParsedDocValidator : AbstractValidator<AddParsedDocCommand>
    {
        public AddParsedDocValidator()
        {
            RuleFor(p => p.FilePath)
                .NotEmpty().WithMessage("FilePath is required.");
        }

    }
}