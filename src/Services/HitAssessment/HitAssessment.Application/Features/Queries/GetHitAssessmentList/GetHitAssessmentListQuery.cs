
using CQRS.Core.Query;
using HitAssessment.Application.Features.Queries.GetHitAssessmentList;
using MediatR;

namespace HitAssessment.Application.Features.Queries.GetHitAssessment.GetHitAssessmentList
{
    public class GetHitAssessmentListQuery : BaseQuery, IRequest<List<HitAssessmentListVM>>
    {
    }
}