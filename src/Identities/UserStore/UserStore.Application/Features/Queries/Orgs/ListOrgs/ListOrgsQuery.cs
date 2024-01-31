
using MediatR;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Queries.Orgs.ListOrgs
{
    public class ListOrgsQuery : IRequest<List<AppOrg>>
    {
        
    }
}