
using FluentValidation;

namespace Project.Application.Features.Commands.DeleteProject
{
    public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
    {
        public DeleteProjectCommandValidator()
        {
            RuleFor(t => t.StrainId)
                .NotEmpty().WithMessage("{StrainId} is required")
                .NotNull();

            
        }
    }
    
}