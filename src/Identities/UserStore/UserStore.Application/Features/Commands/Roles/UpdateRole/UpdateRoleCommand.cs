
using MediatR;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.Roles.UpdateRole
{
    public class UpdateRoleCommand : IRequest<UpdateRoleDTO>
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int SelfAccessLevel { get; set; }
        public int OrganizationAccessLevel { get; set; }
        public int AllAccessLevel { get; set; }
        public List<Guid> Users { get; set; }
    }
}