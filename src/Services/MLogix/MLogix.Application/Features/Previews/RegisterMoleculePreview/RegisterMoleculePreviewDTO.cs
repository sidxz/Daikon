using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLogix.Application.Features.Previews.RegisterMoleculePreview
{
    public class RegisterMoleculePreviewDTO
    {
        public string Name { get; set; }
        public string SMILES { get; set; }
        public List<string> Errors { get; set; } = [];
        public bool IsValid { get; set; }
        public bool IsSMILERegistered { get; set; }
    }
}