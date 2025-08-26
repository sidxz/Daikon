using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Daikon.Shared.APIClients.MLogix
{
    public partial class MLogixAPI
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MLogixAPI> _logger;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MLogixAPI(ILogger<MLogixAPI> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiBaseUrl = configuration["MLogixAPI:Url"] ?? throw new ArgumentNullException(nameof(_apiBaseUrl));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }


        private async Task<T> SendRequestAsync<T>(string apiUrl, HttpMethod method, object content = null)
        {
            try
            {
                var request = new HttpRequestMessage(method, apiUrl);

                var headers = _httpContextAccessor.HttpContext.Request.Headers
                        .ToDictionary(h => h.Key, h => h.Value.ToString());
                        
                request.AddHeaders(headers);
                // Only add content for methods that support a body (POST, PUT, PATCH, etc.)
                if (content != null && (method == HttpMethod.Post || method == HttpMethod.Put || method.Method == "PATCH"))
                {
                    request.Content = new StringContent(JsonSerializer.Serialize(content, _jsonOptions), Encoding.UTF8, "application/json");
                    //_logger.LogDebug("API request content: {Content}", request.Content.ReadAsStringAsync().Result);
                }

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(result, _jsonOptions);
                }
                else
                {
                    _logger.LogWarning("API request failed. URL: {ApiUrl}, Status Code: {StatusCode}", apiUrl, response.StatusCode);
                    return default;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error for URL: {ApiUrl}", apiUrl);
                throw new HttpRequestException("Error while making API request", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing JSON response from URL: {ApiUrl}", apiUrl);
                throw new JsonException("Error parsing response data", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during API request to URL: {ApiUrl}", apiUrl);
                throw new ApplicationException("Unexpected error", ex);
            }
        }

        private string BuildQueryString(IDictionary<string, object> queryParams)
        {
            return string.Join("&", queryParams
                .Where(kvp => kvp.Value != null)
                .Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value.ToString())}"));
        }

    }
}