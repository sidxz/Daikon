using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daikon.Shared.VM.Horizon
{
    public class CompoundRelationsVM
    {
        public Guid Id { get; set; }
        public string NodeType { get; set; } = "";
        public string NodeRelation { get; set; } = "";
        public string NodeName { get; set; } = "";

        // Add properties for any additional node attributes you need
        public Dictionary<string, object> NodeProperties { get; set; } = new Dictionary<string, object>();

    }
}