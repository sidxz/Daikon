using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Daikon.Shared.APIClients.Common
{
    public class APIRequests
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<APIRequests> _logger;

        public APIRequests(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<APIRequests> logger)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            _logger = logger;
        }

        /// <summary>
        /// Sends an HTTP request asynchronously to the specified API URL and returns the deserialized response.
        /// </summary>
        public async Task<T> SendRequestAsync<T>(string apiUrl, HttpMethod method, object content = null, IDictionary<string, object> queryParams = null)
        {
            try
            {
                if (queryParams != null && queryParams.Any())
                {
                    apiUrl = $"{apiUrl}?{BuildQueryString(queryParams)}";
                }

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
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToString());
                }
            }

            // Add content for methods that support a body (POST, PUT, PATCH).
            if (content != null && (method == HttpMethod.Post || method == HttpMethod.Put || method.Method == "PATCH"))
            {
                var jsonContent = JsonSerializer.Serialize(content, _jsonOptions);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
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
