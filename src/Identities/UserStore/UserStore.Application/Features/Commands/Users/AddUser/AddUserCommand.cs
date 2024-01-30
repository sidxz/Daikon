
using MediatR;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.Users.AddUser
{
    public class AddUserCommand : IRequest<AppUser>
    {
        public Guid Id { get; set; }
        public string? OIDCSub { get; set; }
        public string? EntraObjectId { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool? IsSystemAccount { get; set; }
        public Guid AppOrgId { get; set; }
        public List<Guid>? RoleIds { get; set; }

    }
}