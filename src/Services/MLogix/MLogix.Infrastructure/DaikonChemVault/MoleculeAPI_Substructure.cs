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

                // Use reflection to add non-null properties from BaseQueryWithConditionFilters
                var filters = typeof(BaseQueryWithConditionFilters)
                    .GetProperties()
                    .Where(p => p.GetValue(query) != null);

                foreach (var filter in filters)
                {
                    object value = filter.GetValue(query);

                    // Check if the property has a JsonPropertyName attribute
                    var jsonPropertyName = filter.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
                    string name = jsonPropertyName ?? filter.Name;

                    // Add to query params
                    queryParams.Add(name, value);
                }

                // Build the query string
                string queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value.ToString())}"));
                string apiUrl = $"{_apiBaseUrl}/molecules/substructure?{queryString}";

                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.AddHeaders(headers);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var resultMolecules = JsonSerializer.Deserialize<List<MoleculeBase>>(result, _jsonOptions);
                        _logger.LogInformation("Molecules with substructure : {SMILES}", query.SMILES);
                        return resultMolecules;
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Error deserializing JSON response");
                        return null;
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to find substructures. Status Code: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Find Substructures");
                return null;
            }
        }
    }
}