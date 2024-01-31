
using MediatR;
using UserStore.Application.Features.Queries.Users.GetUser.VMs;

namespace UserStore.Application.Features.Queries.Users.ListUsers
{
    public class ListUsersQuery : IRequest<List<AppUserVM>>
    {
        
    }
}