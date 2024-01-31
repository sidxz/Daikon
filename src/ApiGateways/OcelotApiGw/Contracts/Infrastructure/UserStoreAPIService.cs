

using System.Text;
using System.Text.Json;
using OcelotApiGw.DTOs;

namespace OcelotApiGw.Contracts.Infrastructure
{
    public class UserStoreAPIService : IUserStoreAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserStoreAPIService> _logger;
        private readonly string _userStoreApiBaseUrl;
        private readonly string _userStoreAPIValidateUrl;

        public UserStoreAPIService(ILogger<UserStoreAPIService> logger, IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userStoreApiBaseUrl = configuration.GetSection("UserStoreAPI:BaseUrl")?.Value ?? throw new ArgumentNullException(nameof(_userStoreApiBaseUrl));
            _userStoreAPIValidateUrl = configuration.GetSection("UserStoreAPI:Validate")?.Value ?? throw new ArgumentNullException(nameof(_userStoreAPIValidateUrl));
        }
        public async Task<ValidateUserAccessResponse> Validate(string oidcSub, string entraObjectId, string email)
        {
            var validateUserAccessRequest = new ValidateUserAccessRequest
            {
                Email = email
            };

            // set oidcSub if provided
            if (!string.IsNullOrEmpty(oidcSub))
            {
                validateUserAccessRequest.OIDCSub = oidcSub;
            }

            // set entraObjectId if provided
            if (!string.IsNullOrEmpty(entraObjectId))
            {
                validateUserAccessRequest.EntraObjectId = entraObjectId;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true // Useful for debugging, set to false in production for efficiency
            };

            var content = new StringContent(JsonSerializer.Serialize(validateUserAccessRequest, jsonOptions), Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(_userStoreApiBaseUrl + _userStoreAPIValidateUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var resultUserAccess = JsonSerializer.Deserialize<ValidateUserAccessResponse>(result, jsonOptions);
                    _logger.LogInformation("User access validated with email: {email}", resultUserAccess?.Email);
                    return resultUserAccess;
                }
                else
                {
                    // Handle non-success status code
                    _logger.LogWarning("Failed to validate user access. Status Code: {StatusCode}", response.StatusCode);
                    return new ValidateUserAccessResponse { IsValid = false };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate user access");
                return new ValidateUserAccessResponse { IsValid = false }; ;
            }
        }
    }
}