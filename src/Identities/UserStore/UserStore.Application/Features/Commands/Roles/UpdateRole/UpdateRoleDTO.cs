using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserStore.Application.Features.Commands.Roles.UpdateRole
{
    public class UpdateRoleDTO
    {
        public List<Guid> Users { get; set; }
    }
}