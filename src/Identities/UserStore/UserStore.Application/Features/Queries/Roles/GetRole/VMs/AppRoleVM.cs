using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace UserStore.Application.Features.Queries.Roles.GetRole.VMs
{
    public class AppRoleVM : IdentityRole<Guid>
    {
        public string Description { get; set; }
        public int SelfAccessLevel { get; set; }
        public int OrganizationAccessLevel { get; set; }
        public int AllAccessLevel { get; set; }
        public List<Guid> Users { get; set; }
    }
}