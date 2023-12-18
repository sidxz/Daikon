using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Screen.Application.Features.Commands.NewHitCollection
{
    public class NewHitCollectionCommandValidator : AbstractValidator<NewHitCollectionCommand>
    {

        public NewHitCollectionCommandValidator()
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