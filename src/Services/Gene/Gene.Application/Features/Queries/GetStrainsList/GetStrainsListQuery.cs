
using CQRS.Core.Query;
using MediatR;

namespace Gene.Application.Features.Queries.GetStrainsList
{
    public class GetStrainsListQuery : BaseQuery, IRequest<List<StrainsListVM>>
    {
        
    }
}