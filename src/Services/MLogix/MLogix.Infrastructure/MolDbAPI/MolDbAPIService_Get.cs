
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure;
using MLogix.Application.DTOs.MolDbAPI;

namespace MLogix.Infrastructure.MolDbAPI
{
    public partial class MolDbAPIService : IMolDbAPIService
    {
       public async Task<MoleculeDTO> FindExact(string smiles)
        {
            try {
                HttpResponseMessage response = await _httpClient.GetAsync(_molDbApiUrl + "/molecule/find-exact/" + smiles);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var resultMolecule = JsonSerializer.Deserialize<MoleculeDTO>(result, _jsonOptions);
                    _logger.LogInformation("Molecule found by SMILES: {MoleculeId}", resultMolecule?.Id);
                    return resultMolecule;
                }
                else
                {
                    // Handle non-success status code
                    _logger.LogWarning("Failed to find molecule by SMILES. Status Code: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FindExact");
                return null;
            }
        }

        public async Task<List<MoleculeDTO>> FindSimilar(string smiles, float similarityThreshold, int maxResults)
        {
           try {
                string apiUrl = $"{_molDbApiUrl}/molecule/find-similar/{smiles}?threshold={similarityThreshold}&limit={maxResults}";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var resultMolecules = JsonSerializer.Deserialize<List<MoleculeDTO>>(result, _jsonOptions);
                    _logger.LogInformation("Molecules found similar to: {SMILES}", smiles);
                    return resultMolecules;
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

        public async Task<MoleculeDTO> GetMoleculeById(Guid id)
        {
            try {
                HttpResponseMessage response = await _httpClient.GetAsync(_molDbApiUrl + "/molecule/by-id/" + id);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var resultMolecule = JsonSerializer.Deserialize<MoleculeDTO>(result, _jsonOptions);
                    _logger.LogInformation("Molecule retrieved by ID: {MoleculeId}", resultMolecule?.Id);
                    return resultMolecule;
                }
                else
                {
                    // Handle non-success status code
                    _logger.LogWarning("Failed to retrieve molecule by ID. Status Code: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetMoleculeById");
                return null;
            }
        }

        public async Task<MoleculeDTO> GetMoleculeBySMILES(string smiles)
        {
            try {
                HttpResponseMessage response = await _httpClient.GetAsync(_molDbApiUrl + "/molecule/by-smiles/" + smiles);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var resultMolecule = JsonSerializer.Deserialize<MoleculeDTO>(result, _jsonOptions);
                    _logger.LogInformation("Molecule retrieved by SMILES: {MoleculeId}", resultMolecule?.Id);
                    return resultMolecule;
                }
                else
                {
                    // Handle non-success status code
                    _logger.LogWarning("Failed to retrieve molecule by SMILES. Status Code: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetMoleculeBySMILES");
                return null;
            }
                
        }

        public async Task<List<MoleculeDTO>> ListMolecules()
        {
            try {
                HttpResponseMessage response = await _httpClient.GetAsync(_molDbApiUrl + "/molecules");


                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var resultMolecules = JsonSerializer.Deserialize<List<MoleculeDTO>>(result, _jsonOptions);
                    _logger.LogInformation("Molecules listed: {MoleculeCount}", resultMolecules?.Count);
                    return resultMolecules;
                }
                else
                {
                    // Handle non-success status code
                    _logger.LogWarning("Failed to list molecules. Status Code: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ListMolecules");
                return null;
            }
        }
    }
}