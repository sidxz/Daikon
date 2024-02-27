
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure;
using MLogix.Application.DTOs.MolDbAPI;

namespace MLogix.Infrastructure.MolDbAPI
{
    public class MolDbAPIService : IMolDbAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MolDbAPIService> _logger;
        private readonly string _molDbApiUrl;
        public MolDbAPIService(ILogger<MolDbAPIService> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _molDbApiUrl = Environment.GetEnvironmentVariable("MolDbAPI:Url") ?? throw new ArgumentNullException(nameof(_molDbApiUrl));
        }

        public async Task<CompoundDTO> RegisterCompound(string name, string initialCompoundStructure)
        {
            _logger.LogInformation("+++++++++++++++++++++++++++++++++++++++++REGISTER COMPOUND+++++++++++++++++++++++++++++++++++++++++++++++");
            if (string.IsNullOrEmpty(initialCompoundStructure))
                throw new ArgumentNullException(nameof(name));

            var compoundData = new CompoundDTO
            {
                Name = name,
                Smiles = initialCompoundStructure
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true // Useful for debugging, set to false in production for efficiency
            };

            var content = new StringContent(JsonSerializer.Serialize(compoundData, jsonOptions), Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(_molDbApiUrl + "/molecule/register", content);


                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var resultCompound = JsonSerializer.Deserialize<CompoundDTO>(result, jsonOptions);
                    _logger.LogInformation("Compound registered with id: {CompoundId}", resultCompound?.Id);
                    return resultCompound;
                }
                else
                {
                    // Handle non-success status code
                    _logger.LogWarning("Failed to register compound. Status Code: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling MolDbAPI");
                _logger.LogError(ex.Message);
                return null;
            }
        }
    }
}