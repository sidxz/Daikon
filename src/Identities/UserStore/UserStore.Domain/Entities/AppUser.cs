
using Microsoft.AspNetCore.Identity;

namespace UserStore.Domain.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OIDCSub { get; set; }
        public bool IsSystemAccount { get; set; }
        public AppOrg Org { get; set; }
        public List<AppRole> UserRoles { get; set; }
        public bool IsUserLocked { get; set; }
        public bool IsUserArchived { get; set; }
        public DateTime? ArchivedDate { get; set; }

        public bool HasFirstLogin { get; set; }
        public DateTime? FirstLoginDate { get; set; }

        public DateTime? LastLoginDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }

    }
}