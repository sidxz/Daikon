
using CQRS.Core.Query;
using MediatR;
using Target.Application.Features.Queries.GetTargetsList;

namespace Target.Application.Features.Queries.GetTarget.GetTargetsList
{
    public class GetTargetsListQuery : BaseQuery, IRequest<List<TargetsListVM>>
    {
    }
}