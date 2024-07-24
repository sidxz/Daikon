using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Target.Application.Features.Commands.UpdateToxicology
{
    public class UpdateToxicologyValidator : AbstractValidator<UpdateToxicologyCommand>
    {
        public UpdateToxicologyValidator()
        {
            
        }

    }
}