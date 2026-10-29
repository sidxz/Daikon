using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace SimpleGW.Test;

// Even if a service (or anything between it and the gateway) tries to enable caching, the client must get no-store
public class GatewayCachingTests
{
    private const string TenantId = "11111111-1111-1111-1111-111111111111";
    private const string Audience = "api://22222222-2222-2222-2222-222222222222";
    private static readonly string Issuer = $"https://login.microsoftonline.com/{TenantId}/v2.0";
    private static readonly RsaSecurityKey SigningKey = new(RSA.Create(2048)) { KeyId = "gw-test-key" };

    [Fact]
    public async Task Cacheable_responses_from_a_service_reach_the_client_as_no_store()
    {
        using var gateway = new WebApplicationFactory<Program>().WithWebHostBuilder(host =>
        {
            host.UseEnvironment("Testing");
            host.UseSetting("OIDCProvider", "EntraID");
            host.UseSetting("EntraID:Instance", "https://login.microsoftonline.com/");
            host.UseSetting("EntraID:Domain", "example.onmicrosoft.com");
            host.UseSetting("EntraID:TenantId", TenantId);
            host.UseSetting("EntraID:ClientId", "22222222-2222-2222-2222-222222222222");
            host.UseSetting("EntraID:Audience", Audience);
            host.UseSetting("Cors:AllowedOrigins", "https://daikon.example.org");
            host.ConfigureLogging(logging => logging.ClearProviders());
            host.ConfigureTestServices(services =>
            {
                // Real token validation against a local key instead of Entra's published keys
                services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    var metadata = new OpenIdConnectConfiguration { Issuer = Issuer };
                    metadata.SigningKeys.Add(SigningKey);
                    options.Configuration = metadata;
                    options.ConfigurationManager = new StaticConfigurationManager<OpenIdConnectConfiguration>(metadata);
                    options.TokenValidationParameters.ValidIssuers = new[] { Issuer };
                });
                // The "service" behind the gateway does everything it can to make its response cacheable
                services.AddHttpClient("SimpleGWClient").ConfigurePrimaryHttpMessageHandler(() => new CacheHappyService());
            });
        });

        var request = new HttpRequestMessage(HttpMethod.Get, "/v2/gene");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token());
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

    private static string Token() => new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
    {
        Issuer = Issuer,
        Audience = Audience,
        Claims = new Dictionary<string, object> { ["oid"] = "user-oid", ["tid"] = TenantId, ["scp"] = "access_as_user" },
        Expires = DateTime.UtcNow.AddMinutes(30),
        SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.RsaSha256),
    });

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
