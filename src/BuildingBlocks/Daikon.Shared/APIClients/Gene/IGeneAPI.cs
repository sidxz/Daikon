using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.Shared.VM.Gene;

namespace Daikon.Shared.APIClients.Gene
{
    public interface IGeneAPI
    {
        public Task<GeneLiteVM> GetBasicById(Guid id, bool forceRefresh = false);
    }
}