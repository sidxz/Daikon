

namespace OcelotApiGw.DTOs
{
    public class ValidateUserAccessRequest
    {
        public string? OIDCSub { get; set; }
        public string? EntraObjectId { get; set; }
        public string? Email { get; set; }
    }
}