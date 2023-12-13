
using FluentValidation;

namespace Gene.Application.Features.Command.UpdateResistanceMutation
{
    public class UpdateResistanceMutationCommandValidator : AbstractValidator<UpdateResistanceMutationCommand>
    {
        public UpdateResistanceMutationCommandValidator()
        {
             RuleFor(e => e.Mutation)
            .NotEmpty().WithMessage("{Mutation} is required")
            .NotNull();

            RuleFor(e => e.ResistanceMutationId)
            .NotEmpty().WithMessage("{ResistanceMutationId} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{ResistanceMutationId} is required");
        }
    }
}