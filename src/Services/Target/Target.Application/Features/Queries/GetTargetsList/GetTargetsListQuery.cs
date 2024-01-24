
using CQRS.Core.Query;
using MediatR;

namespace Target.Application.Features.Queries.GetTargetsList
{
    public class GetTargetsListQuery : BaseQuery, IRequest<List<TargetsListVM>>
    {
    }
}