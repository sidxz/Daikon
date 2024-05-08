using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Project.Application.Features.Commands.RenameProject
{
    public class RenameProjectValidator : AbstractValidator<RenameProjectCommand>
    {
        public RenameProjectValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
        
    }
}