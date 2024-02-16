
namespace UserStore.Application.Features.Commands.Users.ResolvePermission
{
    public class ResolvePermissionResponse
    {
        public required string AccessLevelDescriptor { get; set; }
        public required string AccessLevel { get; set; }
    }
}