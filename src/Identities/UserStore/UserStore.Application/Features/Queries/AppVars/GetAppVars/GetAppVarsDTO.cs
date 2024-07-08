using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserStore.Application.Features.Queries.AppVars.GetAppVars
{
    public class GetAppVarsDTO
    {
        public Dictionary<Guid, string> UserEmails { get; set; }
        public Dictionary<Guid, string> UserNames { get; set; }
        public Dictionary<Guid, string> Orgs { get; set; }
        public Dictionary<Guid, string> OrgsAlias { get; set; }

        public Dictionary<Guid, string> OrgsVisible { get; set; }
        public Dictionary<Guid, string> OrgsAliasVisible { get; set; }
        public Dictionary<Guid, string> Roles { get; set; }

        public Dictionary<Guid, string> UsersRoles { get; set; }
        public Tuple<Guid, string> UsersOrg { get; set; }
    }
}