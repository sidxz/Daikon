
using Microsoft.Extensions.Logging;
using MLogix.Application.DTOs.DaikonChemVault;
using MLogix.Application.Features.Queries.FindSimilarMolecules;

namespace MLogix.Infrastructure.DaikonChemVault
{
    public partial class MoleculeAPI
    {
        public async Task<List<SimilarMolecule>> FindSimilar(FindSimilarMoleculesQuery query, IDictionary<string, string> headers)
        {
            try
            {

                var queryParams = new Dictionary<string, object>
                {
                    { "smiles", query.SMILES },
                    { "threshold", query.Threshold },
                    { "limit", query.Limit }
                };

                AddConditionFilters(query, queryParams);

                // Build the query string
                string queryString = BuildQueryString(queryParams);
                string apiUrl = $"{_apiBaseUrl}/molecules/similarity?{queryString}";

                var similarMolecules = await SendRequestAsync<List<SimilarMolecule>>(apiUrl, HttpMethod.Get, headers);

                return similarMolecules;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FindSimilar");
                return null;
            }
        }
    }
}