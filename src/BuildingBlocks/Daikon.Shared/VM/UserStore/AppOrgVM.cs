using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace Daikon.Shared.VM.UserStore
{
    public class AppOrgVM : VMMeta
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool IsInternal { get; set; }
        public DateTime? LastEditAt { get; set; }
        public string LastEditBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}