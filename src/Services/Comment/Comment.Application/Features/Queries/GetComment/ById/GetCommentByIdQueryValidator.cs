using FluentValidation;

namespace Comment.Application.Features.Queries.GetComment.ById
{
    public class GetCommentByIdQueryValidator : AbstractValidator<GetCommentByIdQuery>
    {
        public GetCommentByIdQueryValidator()
        {
            RuleFor(g => g.Id)
            .NotEmpty().WithMessage("{Id} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{Id} is required");
        }
        
    }
}