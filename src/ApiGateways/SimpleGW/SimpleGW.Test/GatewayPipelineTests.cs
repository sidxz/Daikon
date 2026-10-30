using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SimpleGW.Test;

// Boots the real gateway (Program.cs middleware order, CORS policy, auth) in memory
public class GatewayPipelineTests : IDisposable
{
    private const string Frontend = TestGateway.Frontend;

    private readonly WebApplicationFactory<Program> _gateway = new WebApplicationFactory<Program>().WithWebHostBuilder(host => TestGateway.Configure(host));

    public void Dispose() => _gateway.Dispose();

    [Fact]
    public async Task Requests_without_a_token_get_a_standard_401_with_security_headers()
    {
        var response = await _gateway.CreateClient().GetAsync("/v2/gene");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Bearer", response.Headers.WwwAuthenticate.ToString());
        AssertSecurityHeaders(response);
    }

    [Fact]
    public async Task Service_health_page_needs_a_signed_in_user()
    {
        var response = await _gateway.CreateClient().GetAsync("/health/services");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Only_the_configured_frontend_origin_passes_CORS()
    {
        var client = _gateway.CreateClient();

        var allowed = await client.SendAsync(Preflight(Frontend, "authorization"));
        Assert.Equal(Frontend, allowed.Headers.GetValues("Access-Control-Allow-Origin").Single());
        AssertSecurityHeaders(allowed);

        var otherOrigin = await client.SendAsync(Preflight("https://not-daikon.example", "authorization"));
        Assert.False(otherOrigin.Headers.Contains("Access-Control-Allow-Origin"));

        // The browser blocks a header the preflight answer doesn't list
        var unlistedHeader = await client.SendAsync(Preflight(Frontend, "x-appuser-id"));
        var listedHeaders = unlistedHeader.Headers.TryGetValues("Access-Control-Allow-Headers", out var values) ? string.Join(",", values) : "";
        Assert.DoesNotContain("x-appuser-id", listedHeaders, StringComparison.OrdinalIgnoreCase);
    }

    private static HttpRequestMessage Preflight(string origin, string requestHeaders)
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/v2/gene");
        request.Headers.Add("Origin", origin);
        request.Headers.Add("Access-Control-Request-Method", "GET");
        request.Headers.Add("Access-Control-Request-Headers", requestHeaders);
        return request;
    }

    private static void AssertSecurityHeaders(HttpResponseMessage response)
    {
        Assert.Equal("no-store, max-age=0", response.Headers.CacheControl!.ToString());
        Assert.Equal("default-src 'none'; frame-ancestors 'none'", response.Headers.GetValues("Content-Security-Policy").Single());
        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").Single());
    }
}
