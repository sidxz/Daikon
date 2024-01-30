using Microsoft.AspNetCore.Identity;

namespace UserStore.Domain.Entities
{
    public class AppRole : IdentityRole<Guid>
    {
        public string Description { get; set; }
        
    }
}