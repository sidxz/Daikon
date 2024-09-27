using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CQRS.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using MLogix.Application.DTOs.DaikonChemVault;
using MLogix.Application.Features.Queries.Filters;
using MLogix.Application.Features.Queries.FindSimilarMolecules;
using MLogix.Application.Features.Queries.FindSubstructures;

namespace MLogix.Infrastructure.DaikonChemVault
{
    public partial class MoleculeAPI
    {
        public async Task<List<MoleculeBase>> FindSubstructure(FindSubstructuresQuery query, IDictionary<string, string> headers)
        {
            try
            {
                // string decodedSmiles = Uri.UnescapeDataString(query.SMILES);
                // string encodedSmiles = Uri.EscapeDataString(decodedSmiles);

                // Build the base query parameters
                var queryParams = new Dictionary<string, object>
                {
                    { "smiles", query.SMILES },
                    { "limit", query.Limit }
                };

                AddConditionFilters(query, queryParams);

                // Build the query string
                string queryString = BuildQueryString(queryParams);
                string apiUrl = $"{_apiBaseUrl}/molecules/substructure?{queryString}";

                var substructures = await SendRequestAsync<List<MoleculeBase>>(apiUrl, HttpMethod.Get, headers);

                return substructures;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Find Substructures");
                return null;
            }
        }
    }
}