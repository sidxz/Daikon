
using MediatR;
using UserStore.Application.Features.Queries.Roles.GetRole.VMs;

namespace UserStore.Application.Features.Queries.Roles.GetRole.ById
{
    public class GetRoleByIdQuery : IRequest<AppRoleVM>
    {
        public Guid Id { get; set; }
    }
}