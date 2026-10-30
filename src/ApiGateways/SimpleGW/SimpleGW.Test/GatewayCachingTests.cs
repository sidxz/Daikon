using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Daikon.ApiHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace SimpleGW.Test;

// Even if a service (or anything between it and the gateway) tries to enable caching, the client must get no-store
public class GatewayCachingTests
{
    [Fact]
    public async Task Cacheable_responses_from_a_service_reach_the_client_as_no_store()
    {
        using var gateway = new WebApplicationFactory<Program>().WithWebHostBuilder(host => TestGateway.Configure(host, services =>
        {
            services.AddScoped<IAppUserResolver, TestGateway.KnownUser>();
            // The "service" behind the gateway does everything it can to make its response cacheable
            services.AddHttpClient("SimpleGWClient").ConfigurePrimaryHttpMessageHandler(() => new CacheHappyService());
        }));

        var request = new HttpRequestMessage(HttpMethod.Get, "/v2/gene");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TestGateway.Token());
        var response = await gateway.CreateClient().SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("{\"genes\":[]}", await response.Content.ReadAsStringAsync());
        Assert.Equal("no-store, max-age=0", response.Headers.CacheControl!.ToString());
        Assert.Equal("no-cache", response.Headers.Pragma.ToString());
        foreach (var name in new[] { "ETag", "Age", "Vary", "Surrogate-Control", "CDN-Cache-Control", "Cloudflare-CDN-Cache-Control" })
        {
            Assert.False(response.Headers.Contains(name), name);
        }
        Assert.Null(response.Content.Headers.Expires);
        Assert.Null(response.Content.Headers.LastModified);
    }

    private class CacheHappyService : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"genes\":[]}", Encoding.UTF8, "application/json"),
            };
            response.Headers.CacheControl = CacheControlHeaderValue.Parse("public, max-age=31536000, immutable");
            response.Headers.ETag = new EntityTagHeaderValue("\"v1\"");
            response.Headers.Age = TimeSpan.FromSeconds(10);
            response.Headers.Vary.Add("Accept");
            response.Headers.TryAddWithoutValidation("Surrogate-Control", "max-age=31536000");
            response.Headers.TryAddWithoutValidation("CDN-Cache-Control", "max-age=31536000");
            response.Headers.TryAddWithoutValidation("Cloudflare-CDN-Cache-Control", "max-age=31536000");
            response.Content.Headers.Expires = DateTimeOffset.UtcNow.AddYears(1);
            response.Content.Headers.LastModified = DateTimeOffset.UtcNow.AddDays(-1);
            return Task.FromResult(response);
        }
    }
}
