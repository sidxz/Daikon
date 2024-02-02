
using MediatR;

namespace UserStore.Application.Features.Commands.Users.ResolvePermission
{
    public class ResolvePermissionQuery : IRequest<ResolvePermissionResponse>
    {
        public Guid UserId { get; set; }
        public required string Method { get; set; }
        public required string Endpoint { get; set; }
    }
}