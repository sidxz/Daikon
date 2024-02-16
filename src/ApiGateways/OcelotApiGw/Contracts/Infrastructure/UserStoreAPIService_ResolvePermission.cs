

using System.Text;
using System.Text.Json;
using OcelotApiGw.DTOs;

namespace OcelotApiGw.Contracts.Infrastructure
{
    public partial class UserStoreAPIService : IUserStoreAPIService
    {  
        public async Task<ResolvePermissionResponse> ResolvePermission(string appUserId, string method, string endpoint)
        {
            var resolvePermissionRequest = new ResolvePermissionRequest
            {
                Method = method,
                Endpoint = endpoint
            };

            // parse the appUserId to a Guid
            if (!Guid.TryParse(appUserId, out Guid appUserIdGuid))
            {
                throw new ArgumentException("Invalid appUserId", nameof(appUserId));
            }
            resolvePermissionRequest.UserId = appUserIdGuid;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true // Useful for debugging, set to false in production for efficiency
            };

            var content = new StringContent(JsonSerializer.Serialize(resolvePermissionRequest, jsonOptions), Encoding.UTF8, "application/json");
            var fullUrl = $"{_userStoreApiBaseUrl.TrimEnd('/')}/{_userStoreAPIResolvePermissionUrl.TrimStart('/')}"; // Ensure proper URL formation

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(fullUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var resultPermission = JsonSerializer.Deserialize<ResolvePermissionResponse>(result, jsonOptions);
                    _logger.LogInformation("Permission resolved for user: {UserId} {MaxPermission}", appUserIdGuid, resultPermission?.AccessLevel);
                    return resultPermission;
                }
                else
                {
                    // Handle non-success status code
                    _logger.LogWarning("Failed to resolve permission. Status Code: {StatusCode}", response.StatusCode);
                    return new ResolvePermissionResponse
                    {
                        AccessLevelDescriptor = "UNKNOWN",
                        AccessLevel = "000"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resolve permission");
                return new ResolvePermissionResponse
                {
                    AccessLevelDescriptor = "UNKNOWN",
                    AccessLevel = "000"
                };
            }

        }
    }
}