
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using HitAssessment.Application.Contracts.Infrastructure;
using HitAssessment.Application.DTOs.MLogixAPI;

namespace HitAssessment.Infrastructure.MLogixAPI
{
    public class MLogixAPIService : IMLogixAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MLogixAPIService> _logger;
        private readonly string _MLogixApiUrl;
        public MLogixAPIService(ILogger<MLogixAPIService> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _MLogixApiUrl = Environment.GetEnvironmentVariable("MLogixAPI:Url") ?? throw new ArgumentNullException(nameof(_MLogixApiUrl));
        }

        public async Task<RegisterMoleculeResponseDTO> RegisterCompound(RegisterMoleculeRequest registerMoleculeRequest)
        {
            _logger.LogInformation("+++++++++++++++++++++++++++++++++++++++++REGISTER COMPOUND+++++++++++++++++++++++++++++++++++++++++++++++");
            if (string.IsNullOrEmpty(registerMoleculeRequest.RequestedSMILES))
                throw new ArgumentNullException(nameof(registerMoleculeRequest.RequestedSMILES));

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true // Useful for debugging, set to false in production for efficiency
            };

            var content = new StringContent(JsonSerializer.Serialize(registerMoleculeRequest, jsonOptions), Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(_MLogixApiUrl + "/molecule/", content);


                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var resultCompound = JsonSerializer.Deserialize<RegisterMoleculeResponseDTO>(result, jsonOptions);
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
                _logger.LogError(ex, "Error while calling MLogixAPI");
                _logger.LogError(ex.Message);
                return null;
            }
        }
    }
}