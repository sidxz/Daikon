using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace UserStore.Application.Features.Commands.APIResources.AddAPIResource
{
    public class AddAPIResourceValidator : AbstractValidator<AddAPIResourceCommand>
    {
        public AddAPIResourceValidator()
        {
            RuleFor(x => x.Endpoint).NotEmpty();
        }
        
    }
}