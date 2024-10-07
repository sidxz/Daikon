using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.DTOs.DaikonChemVault;

namespace MLogix.Infrastructure.DaikonChemVault
{
    public partial class MoleculeAPI : IMoleculeAPI
    {

        public async Task<MoleculeBase> GetMoleculeById(Guid registrationId, IDictionary<string, string> headers)
        {
            string apiUrl = $"{_apiBaseUrl}/molecules/by-id/{registrationId}";
            var molecule = await SendRequestAsync<MoleculeBase>(apiUrl, HttpMethod.Get, headers);
            return molecule;
        }

        public async Task<List<MoleculeBase>> GetMoleculesByIds(List<Guid> registrationIds, IDictionary<string, string> headers)
        {
            string idsQuery = string.Join("&", registrationIds.Select(id => $"ids={id}"));
            string apiUrl = $"{_apiBaseUrl}/molecules/by-ids?{idsQuery}";
            var molecules = await SendRequestAsync<List<MoleculeBase>>(apiUrl, HttpMethod.Get, headers);
            return molecules;
        }

        public async Task<MoleculeBase> GetMoleculeBySMILES(string smiles, IDictionary<string, string> headers)
        {
            string apiUrl = $"{_apiBaseUrl}/molecules/by-smiles-canonical/?smiles={Uri.EscapeDataString(smiles)}";
            var molecule = await SendRequestAsync<MoleculeBase>(apiUrl, HttpMethod.Get, headers);
            return molecule;
        }

        public async Task<List<MoleculeBase>> FindByName(string name, int limit, IDictionary<string, string> headers)
        {
            // Build the query string safely
            var queryParams = new Dictionary<string, object>
            {
                { "name", name },
                { "limit", limit }
            };
            string queryString = BuildQueryString(queryParams);
            string apiUrl = $"{_apiBaseUrl}/molecules/by-name?{queryString}";

            var molecules = await SendRequestAsync<List<MoleculeBase>>(apiUrl, HttpMethod.Get, headers);

            return molecules;
        }

    }
}