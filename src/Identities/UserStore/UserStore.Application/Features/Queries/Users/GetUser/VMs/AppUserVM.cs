
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Queries.Users.GetUser.VMs
{
    public class AppUserVM
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OIDCSub { get; set; }
        public string EntraObjectId { get; set; } // Used by Microsoft Entra as Unique Identifier
        public bool IsSystemAccount { get; set; }
        public Guid AppOrgId { get; set; }
        public AppOrg AppOrg { get; set; }
        public List<Guid> UserRolesId { get; set; }
        public bool IsUserLocked { get; set; }
        public bool IsUserArchived { get; set; }
        public DateTime? ArchivedDate { get; set; }

        public bool IsOIDCConnected { get; set; }
        public DateTime? OIDCConnectionDate { get; set; }

        public DateTime? LastLoginDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
}