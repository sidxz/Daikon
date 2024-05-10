using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace UserStore.Application.Features.Commands.Orgs.AddOrg
{
    public class AddOrgValidator : AbstractValidator<AddOrgCommand>
    {
        public AddOrgValidator()
        {
           
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(500).WithMessage("Name must not exceed 500 characters.");

            RuleFor(command => command.Alias)
                .NotEmpty().WithMessage("Alias is required.")
                .MaximumLength(50).WithMessage("Alias must not exceed 500 characters.");
        }
    }
    
}