
using FluentValidation;
namespace Gene.Application.Features.Command.NewResistanceMutation
{
    public class NewResistanceMutationCommandValidator : AbstractValidator<NewResistanceMutationCommand>
    {
        public NewResistanceMutationCommandValidator()
        {
            RuleFor(e => e.Mutation)
            .NotEmpty().WithMessage("{Mutation} is required")
            .NotNull();

            RuleFor(e => e.GeneId)
            .NotEmpty().WithMessage("{GeneId} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{GeneId} is required");
        }
    }
}