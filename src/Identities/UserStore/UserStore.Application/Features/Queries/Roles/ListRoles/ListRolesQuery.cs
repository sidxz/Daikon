
using MediatR;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Queries.Roles.ListRoles
{
    public class ListRolesQuery : IRequest<List<AppRole>>
    {
        
    }
}