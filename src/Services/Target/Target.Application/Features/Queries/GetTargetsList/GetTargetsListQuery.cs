using MediatR;

namespace Target.Application.Features.Queries.GetTargetsList
{
    public class GetTargetsListQuery : IRequest<List<TargetsListVM>>
    {
        
    }
}