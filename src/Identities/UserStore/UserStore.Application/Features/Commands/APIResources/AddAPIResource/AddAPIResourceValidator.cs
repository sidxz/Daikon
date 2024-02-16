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
            RuleFor(x => x.Endpoint).NotEmpty()
            .WithMessage("Endpoint is required");
            
            RuleFor(x => x.Method).NotEmpty()
            .Must(x => x == "GET" || x == "POST" || x == "PUT" || x == "DELETE")
            .WithMessage("Method must be one of the following: GET, POST, PUT, DELETE");


        }

    }
}