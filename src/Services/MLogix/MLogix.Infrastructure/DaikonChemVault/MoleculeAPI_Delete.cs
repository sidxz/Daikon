using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace MLogix.Infrastructure.DaikonChemVault
{
    public partial class MoleculeAPI
    {
        public async Task<Unit> Delete(Guid registrationId, IDictionary<string, string> headers)
        {
            string apiUrl = $"{_apiBaseUrl}/molecules/{registrationId}";
            var molecule = await SendRequestAsync<Unit>(apiUrl, HttpMethod.Delete, headers);
            return molecule;
        }
    }
}