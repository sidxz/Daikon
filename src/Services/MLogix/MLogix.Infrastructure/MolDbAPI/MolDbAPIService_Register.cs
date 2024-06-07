
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure;
using MLogix.Application.DTOs.MolDbAPI;

namespace MLogix.Infrastructure.MolDbAPI
{
    public partial class MolDbAPIService : IMolDbAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MolDbAPIService> _logger;
        private readonly string _molDbApiUrl;
        private readonly JsonSerializerOptions _jsonOptions;
        public MolDbAPIService(ILogger<MolDbAPIService> logger, IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            //_molDbApiUrl = Environment.GetEnvironmentVariable("MolDbAPI:Url") ?? throw new ArgumentNullException(nameof(_molDbApiUrl));
            _molDbApiUrl = configuration["MolDbAPI:Url"] ?? throw new ArgumentNullException(nameof(_molDbApiUrl));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true // Useful for debugging, set to false in production for efficiency
            };
        }

        public async Task<MoleculeDTO> RegisterCompound(string name, string initialCompoundStructure, IDictionary<string, string> headers)
        {
            _logger.LogInformation("+++++++++++++++++++++++++++++++++++++++++REGISTER COMPOUND+++++++++++++++++++++++++++++++++++++++++++++++");
            if (string.IsNullOrEmpty(initialCompoundStructure))
                throw new ArgumentNullException(nameof(name));

            var compoundData = new MoleculeDTO
            {
                Name = name,
                Smiles = initialCompoundStructure
            };

            var content = new StringContent(JsonSerializer.Serialize(compoundData, _jsonOptions), Encoding.UTF8, "application/json");
            foreach (var header in headers)
            {
                content.Headers.Add(header.Key, header.Value);
            }
            try
            {

                HttpResponseMessage response = await _httpClient.PostAsync(_molDbApiUrl + "/molecule/register", content);


                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var resultCompound = JsonSerializer.Deserialize<MoleculeDTO>(result, _jsonOptions);
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