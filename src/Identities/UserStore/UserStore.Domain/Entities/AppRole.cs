using Microsoft.AspNetCore.Identity;

namespace UserStore.Domain.Entities
{
    public class AppRole : IdentityRole<Guid>
    {
        /*
         Access levels to the resource:
            0 - No Access
            4 - Read Access (Read)
            6 - Write Access (Read, Write)
            7 - Full Access (Read, Write, Delete)

        Access Scopes:
            self - Access to the resource is limited to the user
            organization - Access to the resource is limited to the organization
            all - Access to the resource is not limited
        */
        public string Description { get; set; }
        public int SelfAccessLevel { get; set; }
        public int OrganizationAccessLevel { get; set; }
        public int AllAccessLevel { get; set; }

        

        

    }
}