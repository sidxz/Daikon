using System.Security.Cryptography;
using Daikon.ApiHost;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace SimpleGW.Test;

// The real gateway in memory: Entra settings, real token validation against a local signing key, and a UserStore URL
// that is never called because each test registers the IAppUserResolver it wants.
internal static class TestGateway
{
    public const string Frontend = "https://daikon.example.org";
    private const string TenantId = "11111111-1111-1111-1111-111111111111";
    private const string Audience = "api://22222222-2222-2222-2222-222222222222";
    private static readonly string Issuer = $"https://login.microsoftonline.com/{TenantId}/v2.0";
    private static readonly RsaSecurityKey SigningKey = new(RSA.Create(2048)) { KeyId = "gw-test-key" };

    public static void Configure(IWebHostBuilder host, Action<IServiceCollection>? services = null)
    {
        host.UseEnvironment("Testing"); // keep a developer's local appsettings.Development.json out of the test
        host.UseSetting("OIDCProvider", "EntraID");
        host.UseSetting("EntraID:Instance", "https://login.microsoftonline.com/");
        host.UseSetting("EntraID:Domain", "example.onmicrosoft.com");
        host.UseSetting("EntraID:TenantId", TenantId);
        host.UseSetting("EntraID:ClientId", "22222222-2222-2222-2222-222222222222");
        host.UseSetting("EntraID:Audience", Audience);
        host.UseSetting("Cors:AllowedOrigins", Frontend);
        host.UseSetting("UserStoreAPI:Url", "http://userstore.test/api/v2");
        host.ConfigureLogging(logging => logging.ClearProviders()); // the health poller can't reach any service here
        host.ConfigureTestServices(testServices =>
        {
            // Real token validation against a local key instead of Entra's published keys
            testServices.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var metadata = new OpenIdConnectConfiguration { Issuer = Issuer };
                metadata.SigningKeys.Add(SigningKey);
                options.Configuration = metadata;
                options.ConfigurationManager = new StaticConfigurationManager<OpenIdConnectConfiguration>(metadata);
                options.TokenValidationParameters.ValidIssuers = new[] { Issuer };
            });
            services?.Invoke(testServices);
        });
    }

    // A valid access token for user-oid
    public static string Token() => new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
    {
        Issuer = Issuer,
        Audience = Audience,
        Claims = new Dictionary<string, object> { ["oid"] = "user-oid", ["tid"] = TenantId, ["scp"] = "access_as_user" },
        Expires = DateTime.UtcNow.AddMinutes(30),
        SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.RsaSha256),
    });

    public class KnownUser : IAppUserResolver
    {
        public Task<AppUserIdentity?> Resolve(HttpContext context) =>
            Task.FromResult<AppUserIdentity?>(new AppUserIdentity(Guid.NewGuid(), Guid.NewGuid(), "Known User"));
    }

    public class NotAUser : IAppUserResolver
    {
        public Task<AppUserIdentity?> Resolve(HttpContext context) => Task.FromResult<AppUserIdentity?>(null);
    }

    // UserStore unreachable or failing, as UserStoreAppUserResolver reports it
    public class UserStoreDown : IAppUserResolver
    {
        public Task<AppUserIdentity?> Resolve(HttpContext context) => throw new HttpRequestException("Connection refused (userstore-api:80)");
    }
}
