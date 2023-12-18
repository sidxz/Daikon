
using FluentValidation;


namespace Screen.Application.Features.Commands.UpdateHitCollection
{
    public class UpdateHitCollectionCommandValidator : AbstractValidator<UpdateHitCollectionCommand>
    
    {
        public UpdateHitCollectionCommandValidator()
        {
        
            RuleFor(t => t.HitCollectionId)
                .NotEmpty().WithMessage("{HitCollectionId} is required")
                .NotNull();

            RuleFor(t => t.ScreenId)
                .NotEmpty().WithMessage("{ScreenId} is required")
                .NotNull();
            
            RuleFor(t => t.Name)
                .NotEmpty().WithMessage("{Name} is required")
                .NotNull();
            
            RuleFor(t => t.HitCollectionType)
                .NotEmpty().WithMessage("{HitCollectionType} is required")
                .NotNull(); 
            
            
        
        }

    }
}