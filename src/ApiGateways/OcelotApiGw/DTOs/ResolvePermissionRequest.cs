

namespace OcelotApiGw.DTOs
{
    public class ResolvePermissionRequest
    {
        public Guid UserId { get; set; }
        public required string Method { get; set; }
        public required string Endpoint { get; set; }
    }
}