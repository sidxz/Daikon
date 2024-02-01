using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace UserStore.Application.Features.Commands.APIResources.UpdateAPIResource
{
    public class UpdateAPIResourceValidator : AbstractValidator<UpdateAPIResourceCommand>
    {
        public UpdateAPIResourceValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotEqual(Guid.Empty).WithMessage("{PropertyName} is required.");

            RuleFor(p => p.Endpoint)
                .NotEmpty().WithMessage("{PropertyName} is required.");

        }
        
    }
}