
namespace UserStore.Application.Features.Commands.Users.ValidateUserAccess
{
    public class ValidateUserAccessResponse
    {
        public bool IsValid { get; set; }
        public Guid AppUserId { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid AppOrgId { get; set; }
        public List<Guid> RoleIds { get; set; }
        public bool IsSystemAccount { get; set; }


    }
}