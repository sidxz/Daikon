
using CQRS.Core.Query;
using MediatR;

namespace HitAssessment.Application.Features.Queries.GetHitAssessment.ById
{
    public class GetHitAssessmentByIdQuery : BaseQuery, IRequest<HitAssessmentVM>
    {
        public Guid Id { get; set; }
    }
}