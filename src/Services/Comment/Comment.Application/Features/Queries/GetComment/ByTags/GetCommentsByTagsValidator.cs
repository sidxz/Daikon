using FluentValidation;

namespace Comment.Application.Features.Queries.GetComment.ByTags
{
    public class GetCommentsByTagsValidator : AbstractValidator<GetCommentsByTagsQuery>
    {
        public GetCommentsByTagsValidator()
        {
            RuleFor(g => g.Tags)
            .NotEmpty().WithMessage("Tags are required")
            .NotNull();
        }
        
    }
}