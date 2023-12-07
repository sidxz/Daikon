
using FluentValidation;

namespace Horizon.Application.Features.Command.Gene.AddGene
{
    public class AddGeneToGraphCommandValidator : AbstractValidator<AddGeneToGraphCommand>
    {
        public AddGeneToGraphCommandValidator()
        {
            RuleFor(x => x.AccessionNumber).NotEmpty();
            RuleFor(x => x.GeneId).NotEmpty();
            RuleFor(x => x.StrainId).NotEmpty();
            
        }
    }
}