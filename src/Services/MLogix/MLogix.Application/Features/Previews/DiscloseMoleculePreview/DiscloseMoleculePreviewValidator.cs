
using FluentValidation;

namespace MLogix.Application.Features.Previews.DiscloseMoleculePreview
{
    public class DiscloseMoleculePreviewValidator : AbstractValidator<DiscloseMoleculePreviewQuery>
    {
        public DiscloseMoleculePreviewValidator()
        {
            RuleFor(q => q.Queries)
                .NotEmpty().WithMessage("At least one query must be provided.")
                .Must(q => q != null && q.Count > 0).WithMessage("Query list cannot be empty.");

            RuleForEach(q => q.Queries).SetValidator(new QueryItemValidator());
        }
    }

    public class QueryItemValidator : AbstractValidator<QueryItem>
    {
        public QueryItemValidator()
        {
            RuleFor(q => q.SMILES)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(q => q.Name)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull();
        }
    }
}
