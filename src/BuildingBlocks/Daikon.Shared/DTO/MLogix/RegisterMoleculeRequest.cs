using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;

namespace Daikon.Shared.DTO.MLogix
{
    public class RegisterMoleculeDTO
    {
        public string? Name { get; set; }
        public string SMILES { get; set; }
        public Dictionary<string, string>? Ids { get; set; }
    }
}