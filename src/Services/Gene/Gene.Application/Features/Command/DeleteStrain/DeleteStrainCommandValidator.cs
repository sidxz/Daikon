using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Gene.Application.Features.Command.DeleteStrain
{
    public class DeleteStrainCommandValidator : AbstractValidator<DeleteStrainCommand>
    {
        public DeleteStrainCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("{Id} is required")
                .NotNull().WithMessage("{Id} is required")
                .NotEqual(Guid.Empty).WithMessage("{Id} is required");
        }
        
    }
    
}