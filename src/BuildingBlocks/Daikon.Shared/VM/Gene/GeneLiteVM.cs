using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace Daikon.Shared.VM.Gene
{
    public class GeneLiteVM : VMMeta
    {
        public Guid Id { get; set; }
        public Guid StrainId { get; set; }
        public string AccessionNumber { get; set; }
        public string UniProtKB { get; set; }
        public string Name { get; set; }
        public string ProteinNameExpanded { get; set; }

        public string Product { get; set; }
        public string FunctionalCategory { get; set; }
        public string Comments { get; set; }
    }
}