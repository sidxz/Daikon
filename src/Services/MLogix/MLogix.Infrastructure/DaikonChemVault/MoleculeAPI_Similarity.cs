using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CQRS.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using MLogix.Application.DTOs.DaikonChemVault;

namespace MLogix.Infrastructure.DaikonChemVault
{
    public partial class MoleculeAPI
    {
        public async Task<List<SimilarMolecule>> FindSimilar(string smiles, float similarityThreshold, int maxResults, IDictionary<string, string> headers)
        {
            try
            {
                //_logger.LogInformation("Finding similar molecules to: {SMILES} with threshold: {Threshold} and max results: {MaxResults}", smiles, similarityThreshold, maxResults);
                string decodedSmiles = Uri.UnescapeDataString(smiles);
                string encodedSmiles = Uri.EscapeDataString(decodedSmiles);

                string apiUrl = $"{_apiBaseUrl}/molecules/similarity?smiles={encodedSmiles}&threshold={similarityThreshold}&limit={maxResults}";
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.AddHeaders(headers);
                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    // _logger.LogInformation("Raw result: {Result}", result);
                    try
                    {
                        var resultMolecules = JsonSerializer.Deserialize<List<SimilarMolecule>>(result, _jsonOptions);
                        _logger.LogInformation("Molecules found similar to: {SMILES}", smiles);
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
                    // Handle non-success status code
                    _logger.LogWarning("Failed to find similar molecules. Status Code: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FindSimilar");
                return null;
            }
        }
    }
}