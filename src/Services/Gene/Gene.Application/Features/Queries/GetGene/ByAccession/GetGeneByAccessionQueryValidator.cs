using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Gene.Application.Features.Queries.GetGene.ByAccession
{
    public class GetGeneByAccessionQueryValidator : AbstractValidator<GetGeneByAccessionQuery>
    {
        public GetGeneByAccessionQueryValidator()
        {
            RuleFor(g => g.AccessionNumber)
            .NotEmpty().WithMessage("{AccessionNumber} is required")
            .NotNull();
        }
 
    }
}