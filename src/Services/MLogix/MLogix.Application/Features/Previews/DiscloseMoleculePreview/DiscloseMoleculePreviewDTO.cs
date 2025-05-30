using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLogix.Application.Features.Previews.DiscloseMoleculePreview
{
    public class DiscloseMoleculePreviewDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string RequestedSMILES { get; set; }
        public List<string> Errors { get; set; } = [];
        public bool IsValid { get; set; }
    }
}