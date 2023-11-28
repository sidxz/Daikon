using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Gene.Application.Features.Queries.GetGeneVersions.ById
{
    public class GetGeneVersionsValidator : AbstractValidator<GetGeneVersionsQuery>
    {
        public GetGeneVersionsValidator()
        {
            RuleFor(g => g.Id)
            .NotEmpty().WithMessage("{Id} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{Id} is required");
        }
 
    }
}