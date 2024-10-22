using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Daikon.Shared.APIClients.UserStore
{
    public partial class UserStoreAPI : IUserStoreAPI
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserStoreAPI> _logger;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public UserStoreAPI(IMemoryCache memoryCache, ILogger<UserStoreAPI> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiBaseUrl = configuration["UserStoreAPI:Url"] ?? throw new ArgumentNullException(nameof(_apiBaseUrl));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient)); // Reuse an injected HttpClient instance.
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        /// <summary>
        /// Sends an HTTP request asynchronously to the specified API URL.
        /// </summary>
        private async Task<T> SendRequestAsync<T>(string apiUrl, HttpMethod method, object content = null)
        {
            try
            {
                var request = CreateHttpRequestMessage(apiUrl, method, content);

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
                throw new HttpRequestException("Error while making API request.", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing JSON response from URL: {ApiUrl}", apiUrl);
                throw new JsonException("Error parsing the response data.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during API request to URL: {ApiUrl}", apiUrl);
                throw new ApplicationException("Unexpected error occurred during the API request.", ex);
            }
        }

        /// <summary>
        /// Creates an HttpRequestMessage and sets headers based on the current HTTP context.
        /// </summary>
        private HttpRequestMessage CreateHttpRequestMessage(string apiUrl, HttpMethod method, object content)
        {
            var request = new HttpRequestMessage(method, apiUrl);

            // Copy request headers from the current HTTP context.
            if (_httpContextAccessor.HttpContext?.Request?.Headers != null)
            {
                foreach (var header in _httpContextAccessor.HttpContext.Request.Headers)
                {
                    // Only add the headers that are allowed to be sent through to the external API.
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToString());
                }
            }

            // Add content for methods that support a body (POST, PUT, PATCH).
            if (content != null && (method == HttpMethod.Post || method == HttpMethod.Put || method.Method == "PATCH"))
            {
                var jsonContent = JsonSerializer.Serialize(content, _jsonOptions);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                _logger.LogDebug("API request content: {Content}", jsonContent);
            }

            return request;
        }

        /// <summary>
        /// Builds a query string from the provided dictionary of query parameters.
        /// </summary>
        private string BuildQueryString(IDictionary<string, object> queryParams)
        {
            return string.Join("&", queryParams
                .Where(kvp => kvp.Value != null)
                .Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value.ToString())}"));
        }
    }
}
