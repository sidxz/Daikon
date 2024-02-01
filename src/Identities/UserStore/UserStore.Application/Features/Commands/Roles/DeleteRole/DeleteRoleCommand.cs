
using MediatR;

namespace UserStore.Application.Features.Commands.Roles.DeleteRole
{
    public class DeleteRoleCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}