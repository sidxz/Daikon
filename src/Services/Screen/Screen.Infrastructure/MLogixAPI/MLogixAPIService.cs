
using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Infrastructure;
using Screen.Application.Contracts.Infrastructure.DTOs;
using Screen.Application.DTOs.MLogixAPI;

namespace Screen.Infrastructure.MLogixAPI
{
    public class MLogixAPIService : IMLogixAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MLogixAPIService> _logger;
        private readonly string _MLogixApiUrl;
        private readonly IMapper _mapper;

        private readonly JsonSerializerOptions _jsonOptions;
        public MLogixAPIService(ILogger<MLogixAPIService> logger, IMapper mapper)
        {
            _httpClient = new HttpClient();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _MLogixApiUrl = Environment.GetEnvironmentVariable("MLogixAPI:Url") ?? throw new ArgumentNullException(nameof(_MLogixApiUrl));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true // Useful for debugging, set to false in production for efficiency
            };
        }
        
        public async Task<RegisterMoleculeResponseDTO> RegisterCompound(RegisterMoleculeRequest registerMoleculeRequest)
        {
            _logger.LogInformation("RegisterCompound()");
            if (string.IsNullOrEmpty(registerMoleculeRequest.RequestedSMILES))
                throw new ArgumentNullException(nameof(registerMoleculeRequest.RequestedSMILES));

            var content = new StringContent(JsonSerializer.Serialize(registerMoleculeRequest, _jsonOptions), Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(_MLogixApiUrl + "/molecule/", content);


                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var resultCompound = JsonSerializer.Deserialize<RegisterMoleculeResponseDTO>(result, _jsonOptions);
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

        public async Task<GetMoleculesResultDTO> GetMoleculeById(Guid id)
        {
            _logger.LogInformation("GetMoleculeById()");
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_MLogixApiUrl + "/molecule/" + id);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var resultCompound = JsonSerializer.Deserialize<GetMoleculesResultDTO>(result, _jsonOptions);
                    return resultCompound;
                }
                else
                {
                    // Handle non-success status code
                    _logger.LogWarning("Failed to get compound. Status Code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling MLogixAPI");
                _logger.LogError(ex.Message);
                return null;
            }
            return null;


        }
    }
}