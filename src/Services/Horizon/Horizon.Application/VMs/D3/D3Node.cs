using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SecurityToken.Model;

namespace Horizon.Application.VMs.D3
{
    public class D3Node
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string NeoId { get; set; }
        public Guid Id { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = [];
        public List<D3Node> Children { get; set; } = [];
    }
}