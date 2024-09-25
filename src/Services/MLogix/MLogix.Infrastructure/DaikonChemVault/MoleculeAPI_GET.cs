using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CQRS.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.DTOs.DaikonChemVault;

namespace MLogix.Infrastructure.DaikonChemVault
{
    public partial class MoleculeAPI : IMoleculeAPI
    {
        
        public async Task<MoleculeBase> GetMoleculeById(Guid RegistrationId, IDictionary<string, string> headers)
        {
            try
            {
                string apiUrl = $"{_apiBaseUrl}/molecules/by-id/{RegistrationId}";
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.AddHeaders(headers);
                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var resultMolecule = JsonSerializer.Deserialize<MoleculeBase>(result, _jsonOptions);
                    _logger.LogInformation("Daikon Chem Vault : Molecule retrieved by ID: {MoleculeId}", resultMolecule?.Id);
                    return resultMolecule;
                }
                else
                {
                    // Handle non-success status code
                    _logger.LogWarning("Daikon Chem Vault : Failed to retrieve molecule by ID. Status Code: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetMoleculeById");
                return null;
            }
        }

        public async Task<MoleculeBase> GetMoleculeBySMILES(string smiles, IDictionary<string, string> headers)
        {
            try
            {
                string apiUrl = $"{_apiBaseUrl}/molecules/by-smiles-canonical/?smiles={smiles}";
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.AddHeaders(headers);
                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var resultMolecule = JsonSerializer.Deserialize<MoleculeBase>(result, _jsonOptions);
                    _logger.LogInformation("Daikon Chem Vault : Molecule retrieved by SMILES: {smiles}", resultMolecule?.SmilesCanonical);
                    return resultMolecule;
                }
                else
                {
                    // Handle non-success status code
                    _logger.LogWarning("Daikon Chem Vault : Failed to retrieve molecule by SMILES. Status Code: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetMoleculeBySMILES");
                return null;
            }
        }
    }
}