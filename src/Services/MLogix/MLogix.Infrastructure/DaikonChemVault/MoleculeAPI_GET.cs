using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.DTOs.DaikonChemVault;
using MLogix.Application.Features.Queries.GetMolecule.ByName;

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


        public async Task<List<MoleculeBase>> FindByNameWithFilters(GetByNameQuery query, IDictionary<string, string> headers)
        {
            try
            {
                // string decodedSmiles = Uri.UnescapeDataString(query.SMILES);
                // string encodedSmiles = Uri.EscapeDataString(decodedSmiles);

                // Build the base query parameters
                var queryParams = new Dictionary<string, object>
                {
                    { "name", query.Name },
                    { "limit", query.Limit }
                };

                AddConditionFilters(query, queryParams);

                // Build the query string
                string queryString = BuildQueryString(queryParams);
                string apiUrl = $"{_apiBaseUrl}/molecules/by-name?{queryString}";

                var molecules = await SendRequestAsync<List<MoleculeBase>>(apiUrl, HttpMethod.Get, headers);

                return molecules;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Find Substructures");
                return null;
            }
        }

    }
}