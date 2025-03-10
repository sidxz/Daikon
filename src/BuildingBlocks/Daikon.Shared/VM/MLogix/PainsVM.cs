using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace Daikon.Shared.VM.MLogix
{
    public class PainsVM : VMMeta
    {
        public bool RDKitPains { get; set; }
        public List<string> RDKitPainsLabels { get; set; }

    }
}