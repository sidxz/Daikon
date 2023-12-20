
using FluentValidation;

namespace Horizon.Application.Features.Command.Gene.AddGene
{
    public class AddGeneCommandValidator : AbstractValidator<AddGeneCommand>
    {
        public AddGeneCommandValidator()
        {
            RuleFor(x => x.AccessionNumber).NotEmpty();
            RuleFor(x => x.GeneId).NotEmpty();
            RuleFor(x => x.StrainId).NotEmpty();
            
        }
    }
}