
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using HitAssessment.Application.Contracts.Infrastructure;
using HitAssessment.Application.DTOs.MLogixAPI;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using CQRS.Core.Infrastructure;
namespace HitAssessment.Infrastructure.MLogixAPI
{
    public class MLogixAPIService : IMLogixAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MLogixAPIService> _logger;
        private readonly string _MLogixApiUrl;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly JsonSerializerOptions _jsonOptions;
        public MLogixAPIService(ILogger<MLogixAPIService> logger, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _MLogixApiUrl = configuration["MLogixAPI:Url"] ?? throw new ArgumentNullException(nameof(_MLogixApiUrl));
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

            var headers = _httpContextAccessor.HttpContext.Request.Headers
                        .ToDictionary(h => h.Key, h => h.Value.ToString());


            var content = new StringContent(JsonSerializer.Serialize(registerMoleculeRequest, _jsonOptions), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, _MLogixApiUrl + "/molecule/")
            {
                Content = content
            };
            request.AddHeaders(headers);

            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(request);


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
                string apiUrl = $"{_MLogixApiUrl}/molecule/{id}";
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                var headers = _httpContextAccessor.HttpContext.Request.Headers
                        .ToDictionary(h => h.Key, h => h.Value.ToString());

                request.AddHeaders(headers);
                HttpResponseMessage response = await _httpClient.SendAsync(request);

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