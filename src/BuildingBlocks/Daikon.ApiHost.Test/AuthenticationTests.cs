using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using CQRS.Core.Extensions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Middlewares;
using CQRS.Core.Query;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Daikon.ApiHost.Test;

// Every service is built from AddDaikonApiDefaults + UseDaikonApiDefaults, so this covers all of them.
public class AuthenticationTests
{
    private static readonly Dictionary<string, string?> EntraIdConfig = new()
    {
        ["OIDCProvider"] = "EntraID",
        ["EntraID:Instance"] = "https://login.microsoftonline.com/",
        ["EntraID:Domain"] = "example.onmicrosoft.com",
        ["EntraID:TenantId"] = "11111111-1111-1111-1111-111111111111",
        ["EntraID:ClientId"] = "22222222-2222-2222-2222-222222222222",
        ["EntraID:Audience"] = "api://22222222-2222-2222-2222-222222222222",
    };

    private static readonly AppUserIdentity KnownUser = new(Guid.NewGuid(), Guid.NewGuid(), "Known User");

    private const string TenantId = "11111111-1111-1111-1111-111111111111";
    private static readonly string EntraIssuer = $"https://login.microsoftonline.com/{TenantId}/v2.0";
    private static readonly RsaSecurityKey LocalSigningKey = new(RSA.Create(2048)) { KeyId = "local-test-key" };

    private const string KeyCloakAuthority = "https://keycloak.example.org/realms/daikon";
    private static readonly Dictionary<string, string?> KeyCloakConfig = new()
    {
        ["OIDCProvider"] = "KeyCloak",
        ["KeyCloak:Authority"] = KeyCloakAuthority,
        ["KeyCloak:Audience"] = "daikon-frontend", // same as the frontend's client id: the case where ID tokens match
    };

    public static TheoryData<string, HttpStatusCode> TokenCases => new()
    {
        { "valid", HttpStatusCode.OK },
        { "wrong-audience", HttpStatusCode.Unauthorized },
        { "expired", HttpStatusCode.Unauthorized },
        { "no-scope-or-role", HttpStatusCode.Unauthorized }, // e.g. an ID token
        { "other-tenant", HttpStatusCode.Unauthorized },
        { "unknown-signing-key", HttpStatusCode.Unauthorized },
        { "unsigned-alg-none", HttpStatusCode.Unauthorized },
        { "hs256-shared-secret", HttpStatusCode.Unauthorized }, // algorithm confusion
    };

    [Theory]
    [MemberData(nameof(TokenCases))]
    public async Task Bearer_tokens_get_the_real_Entra_checks(string tokenCase, HttpStatusCode expected)
    {
        var client = await StartService(EntraIdConfig, UseLocalSigningKey(EntraIssuer));
        var token = tokenCase switch
        {
            "wrong-audience" => EntraToken(audience: "api://someone-else"),
            "expired" => EntraToken(expires: DateTime.UtcNow.AddMinutes(-10)),
            "no-scope-or-role" => EntraToken(scope: null),
            "other-tenant" => EntraToken(tenantId: "33333333-3333-3333-3333-333333333333"),
            "unknown-signing-key" => EntraToken(signing: new SigningCredentials(
                new RsaSecurityKey(RSA.Create(2048)) { KeyId = "not-entra" }, SecurityAlgorithms.RsaSha256)),
            "unsigned-alg-none" => EntraToken(unsigned: true),
            "hs256-shared-secret" => EntraToken(signing: new SigningCredentials(
                new SymmetricSecurityKey(RandomNumberGenerator.GetBytes(32)), SecurityAlgorithms.HmacSha256)),
            _ => EntraToken(),
        };

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v2/whoami");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.SendAsync(request);
        Assert.True(response.StatusCode == expected, $"{tokenCase}: {(int)response.StatusCode} {response.Headers.WwwAuthenticate}");
    }

    public static TheoryData<string?, HttpStatusCode> KeyCloakTokenTypes => new()
    {
        { "Bearer", HttpStatusCode.OK },
        { "ID", HttpStatusCode.Unauthorized },     // the user's own ID token
        { "Logout", HttpStatusCode.Unauthorized }, // back-channel logout token, same realm key
        { null, HttpStatusCode.Unauthorized },
    };

    [Theory]
    [MemberData(nameof(KeyCloakTokenTypes))]
    public async Task KeyCloak_accepts_only_access_tokens(string? typ, HttpStatusCode expected)
    {
        var client = await StartService(KeyCloakConfig, UseLocalSigningKey(KeyCloakAuthority));
        var claims = new Dictionary<string, object> { ["sub"] = "known-oid", ["oid"] = "known-oid" };
        if (typ is not null)
        {
            claims["typ"] = typ;
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v2/whoami");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SignedToken(KeyCloakAuthority, "daikon-frontend", claims));
        var response = await client.SendAsync(request);
        Assert.True(response.StatusCode == expected, $"typ={typ}: {(int)response.StatusCode} {response.Headers.WwwAuthenticate}");
    }

    [Fact]
    public async Task Daikon_claims_inside_a_token_cannot_override_the_resolved_user()
    {
        var client = await StartService(EntraIdConfig, UseLocalSigningKey(EntraIssuer));

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v2/whoami");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", EntraToken(extra: new()
        {
            [DaikonClaims.AppUserId] = Guid.NewGuid().ToString(),
            [DaikonClaims.AppOrgId] = Guid.NewGuid().ToString(),
        }));

        Assert.Equal($"{KnownUser.AppUserId} {KnownUser.AppOrgId}", await (await client.SendAsync(request)).Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Unknown_users_are_remembered_briefly_so_UserStore_is_not_asked_on_every_request()
    {
        var resolver = new CountingResolver();
        var client = await StartService(EntraIdConfig, services =>
        {
            UseTestTokens(services);
            services.AddSingleton<IAppUserResolver>(resolver);
        });

        for (var i = 0; i < 3; i++)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v2/whoami");
            request.Headers.Add("Authorization", "Test unknown-oid");
            Assert.Equal(HttpStatusCode.Unauthorized, (await client.SendAsync(request)).StatusCode);
        }
        Assert.Equal(1, resolver.Calls);
    }

    [Fact]
    public async Task Endpoints_reject_requests_without_a_token_but_health_stays_open()
    {
        var client = await StartService(EntraIdConfig);

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health")).StatusCode);

        // Identity headers alone must not be enough
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v2/whoami");
        request.Headers.Add("AppUser-Id", Guid.NewGuid().ToString());
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.SendAsync(request)).StatusCode);
    }

    [Fact]
    public async Task Missing_provider_config_fails_startup()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => StartService(new()));

        var noAudience = new Dictionary<string, string?>(EntraIdConfig) { ["EntraID:Audience"] = "" };
        await Assert.ThrowsAsync<InvalidOperationException>(() => StartService(noAudience));
    }

    [Fact]
    public async Task User_comes_from_the_token_and_forged_identity_headers_are_ignored()
    {
        var client = await StartService(EntraIdConfig, UseTestTokens);

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v2/whoami");
        request.Headers.Add("Authorization", "Test known-oid");
        request.Headers.Add("AppUser-Id", Guid.NewGuid().ToString());
        request.Headers.Add("AppOrg-Id", Guid.NewGuid().ToString());

        var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal($"{KnownUser.AppUserId} {KnownUser.AppOrgId}", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Valid_token_for_someone_who_is_not_a_daikon_user_is_rejected()
    {
        var client = await StartService(EntraIdConfig, UseTestTokens);

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v2/whoami");
        request.Headers.Add("Authorization", "Test unknown-oid");
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.SendAsync(request)).StatusCode);
    }

    [Fact]
    public async Task RequestorUserId_comes_from_the_resolved_user_not_the_header()
    {
        var context = new DefaultHttpContext();
        context.User.AddIdentity(new ClaimsIdentity(new[] { new Claim(DaikonClaims.AppUserId, KnownUser.AppUserId.ToString()) }));
        context.Request.Headers["AppUser-Id"] = Guid.NewGuid().ToString();
        context.Request.Headers["trace-id"] = "<script>not an id</script>";
        var behavior = new RequestorIdBehavior<WhoAmIQuery, Guid>(
            new HttpContextAccessor { HttpContext = context }, NullLogger<RequestorIdBehavior<WhoAmIQuery, Guid>>.Instance);

        var query = new WhoAmIQuery();
        var requestorUserId = await behavior.Handle(query, () => Task.FromResult(query.RequestorUserId), CancellationToken.None);

        Assert.Equal(KnownUser.AppUserId, requestorUserId);
        Assert.DoesNotContain("script", query.TraceId); // stored with documents, so junk is replaced
    }

    [Fact]
    public async Task Every_response_carries_no_store_and_security_headers()
    {
        var client = await StartService(EntraIdConfig);

        foreach (var response in new[] { await client.GetAsync("/health"), await client.GetAsync("/api/v2/whoami") })
        {
            Assert.Equal("no-store, max-age=0", response.Headers.CacheControl!.ToString());
            Assert.Equal("default-src 'none'; frame-ancestors 'none'", response.Headers.GetValues("Content-Security-Policy").Single());
            Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
            Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").Single());
            Assert.Equal("max-age=31536000", response.Headers.GetValues("Strict-Transport-Security").Single());
        }
    }

    [Fact]
    public async Task A_service_cannot_make_its_responses_cacheable()
    {
        var client = await StartService(EntraIdConfig, UseLocalSigningKey(EntraIssuer));
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v2/tries-to-cache");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", EntraToken());

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("no-store, max-age=0", response.Headers.CacheControl!.ToString());
        Assert.Equal("no-cache", response.Headers.Pragma.ToString());
        foreach (var name in new[] { "ETag", "Age", "Surrogate-Control", "CDN-Cache-Control", "Cloudflare-CDN-Cache-Control" })
        {
            Assert.False(response.Headers.Contains(name), name);
        }
        Assert.Null(response.Content.Headers.Expires);
        Assert.Null(response.Content.Headers.LastModified);
    }

    [Fact]
    public void Credentials_never_reach_header_logs()
    {
        var headers = new DefaultHttpContext().Request.Headers;
        headers.Authorization = "Bearer secret-token";
        headers["cookie"] = "session=secret";
        headers["trace-id"] = "abc123";

        var logged = headers.ToDictionary(h => h.Key, h => h.LoggableValue());

        Assert.Equal("[redacted]", logged["Authorization"]);
        Assert.Equal("[redacted]", logged["cookie"]);
        Assert.Equal("abc123", logged["trace-id"]);
    }

    [Fact]
    public void Service_to_service_calls_pass_on_only_the_token_and_trace_id()
    {
        var incoming = new DefaultHttpContext().Request.Headers;
        incoming.Authorization = "Bearer t";
        incoming["trace-id"] = "abc123";
        incoming["Cookie"] = "session=1";
        incoming["Host"] = "screen-api";
        incoming["AppUser-Id"] = Guid.NewGuid().ToString();

        var outgoing = new HttpRequestMessage(HttpMethod.Get, "http://mlogix-api/api/v2/molecule");
        outgoing.AddHeaders(incoming);
        Assert.Equal(new[] { "Authorization", "trace-id" }, outgoing.Headers.Select(h => h.Key).OrderBy(k => k));

        // The dictionary overload, as the MLogix handlers call it for chemvault
        var toChemVault = new HttpRequestMessage(HttpMethod.Get, "http://daikon_chem_vault_api:10001/molecules/");
        toChemVault.AddHeaders(incoming.ToDictionary(h => h.Key, h => h.Value.ToString()));
        Assert.Equal(new[] { "Authorization", "trace-id" }, toChemVault.Headers.Select(h => h.Key).OrderBy(k => k));
    }

    [Fact]
    public async Task UserStore_resolver_sends_the_token_to_the_current_user_endpoint_and_treats_401_as_unknown()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["UserStoreAPI:Url"] = "http://userstore-api/api/v2" })
            .Build();
        var context = new DefaultHttpContext();
        context.Request.Headers.Authorization = "Bearer token-123";

        // Shape of UserStore's GET /api/v2/user/header response
        var ok = new StubHandler(HttpStatusCode.OK,
            $$"""{"id":"{{KnownUser.AppUserId}}","appOrgId":"{{KnownUser.AppOrgId}}","firstName":"Known","lastName":"User","email":"k@example.com"}""");
        Assert.Equal(KnownUser, await new UserStoreAppUserResolver(new StubFactory(ok), config, NullLogger<UserStoreAppUserResolver>.Instance).Resolve(context));
        Assert.Equal("http://userstore-api/api/v2/user/header", ok.LastRequest!.RequestUri!.ToString());
        Assert.Equal("Bearer token-123", ok.LastRequest.Headers.Authorization!.ToString());

        var unauthorized = new StubHandler(HttpStatusCode.Unauthorized, "");
        Assert.Null(await new UserStoreAppUserResolver(new StubFactory(unauthorized), config, NullLogger<UserStoreAppUserResolver>.Instance).Resolve(context));
    }

    private static async Task<HttpClient> StartService(Dictionary<string, string?> config, Action<IServiceCollection>? overrides = null)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Configuration.AddInMemoryCollection(config);
        builder.Services.AddDaikonApiDefaults(builder.Configuration);
        overrides?.Invoke(builder.Services);

        var app = builder.Build();
        app.UseDaikonApiDefaults();
        app.MapGet("/api/v2/whoami", (HttpContext context) => $"{context.User.GetAppUserId()} {context.User.GetAppOrgId()}");
        app.MapGet("/api/v2/tries-to-cache", (HttpContext context) =>
        {
            var headers = context.Response.Headers;
            headers.CacheControl = "public, max-age=31536000, immutable";
            headers.Expires = "Thu, 01 Jan 2099 00:00:00 GMT";
            headers.ETag = "\"v1\"";
            headers.LastModified = "Mon, 01 Jan 2024 00:00:00 GMT";
            headers.Age = "10";
            headers["Surrogate-Control"] = "max-age=31536000";
            headers["CDN-Cache-Control"] = "max-age=31536000";
            headers["Cloudflare-CDN-Cache-Control"] = "max-age=31536000";
            return "cacheable?";
        });
        await app.StartAsync();
        return app.GetTestClient();
    }

    // Real token validation (Microsoft.Identity.Web for Entra, JwtBearer for KeyCloak); only the download of the
    // provider's metadata and signing keys is replaced by a local key, so tokens can be minted here without network access
    private static Action<IServiceCollection> UseLocalSigningKey(string issuer) => services =>
    {
        services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            var metadata = new OpenIdConnectConfiguration { Issuer = issuer };
            metadata.SigningKeys.Add(LocalSigningKey);
            options.Configuration = metadata;
            options.ConfigurationManager = new StaticConfigurationManager<OpenIdConnectConfiguration>(metadata);
            // The issuer Microsoft.Identity.Web's Entra issuer check otherwise reads from Entra's metadata
            options.TokenValidationParameters.ValidIssuers = new[] { issuer };
        });
        services.AddScoped<IAppUserResolver, KnownUserResolver>();
    };

    private static string EntraToken(
        string audience = "api://22222222-2222-2222-2222-222222222222",
        string? scope = "access_as_user",
        string tenantId = TenantId,
        DateTime? expires = null,
        SigningCredentials? signing = null,
        bool unsigned = false,
        Dictionary<string, object>? extra = null)
    {
        var claims = new Dictionary<string, object> { ["oid"] = "known-oid", ["tid"] = tenantId };
        if (scope is not null)
        {
            claims["scp"] = scope;
        }
        foreach (var (type, value) in extra ?? new())
        {
            claims[type] = value;
        }
        return SignedToken($"https://login.microsoftonline.com/{tenantId}/v2.0", audience, claims, expires, signing, unsigned);
    }

    private static string SignedToken(string issuer, string audience, Dictionary<string, object> claims,
        DateTime? expires = null, SigningCredentials? signing = null, bool unsigned = false)
    {
        var expiry = expires ?? DateTime.UtcNow.AddMinutes(30);
        return new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Claims = claims,
            IssuedAt = expiry.AddHours(-1),
            NotBefore = expiry.AddHours(-1),
            Expires = expiry,
            SigningCredentials = unsigned ? null : signing ?? new SigningCredentials(LocalSigningKey, SecurityAlgorithms.RsaSha256),
        });
    }

    // Swaps Entra for "Authorization: Test <oid>" and UserStore for a resolver that knows one user
    private static void UseTestTokens(IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "Test";
            options.DefaultAuthenticateScheme = "Test";
            options.DefaultChallengeScheme = "Test";
        }).AddScheme<AuthenticationSchemeOptions, TestTokenHandler>("Test", null);
        services.AddScoped<IAppUserResolver, KnownUserResolver>();
    }

    private class TestTokenHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestTokenHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var header = Request.Headers.Authorization.ToString();
            if (!header.StartsWith("Test "))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimConstants.ObjectId, header["Test ".Length..]) }, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name)));
        }
    }

    private class KnownUserResolver : IAppUserResolver
    {
        public Task<AppUserIdentity?> Resolve(HttpContext context) =>
            Task.FromResult(context.User.GetObjectId() == "known-oid" ? KnownUser : null);
    }

    private class CountingResolver : IAppUserResolver
    {
        public int Calls;

        public Task<AppUserIdentity?> Resolve(HttpContext context)
        {
            Interlocked.Increment(ref Calls);
            return Task.FromResult<AppUserIdentity?>(null);
        }
    }

    private class WhoAmIQuery : BaseQuery, IRequest<Guid> { }

    private class StubHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _status;
        private readonly string _body;
        public HttpRequestMessage? LastRequest { get; private set; }

        public StubHandler(HttpStatusCode status, string body)
        {
            _status = status;
            _body = body;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(new HttpResponseMessage(_status) { Content = new StringContent(_body, System.Text.Encoding.UTF8, "application/json") });
        }
    }

    private class StubFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler;
        public StubFactory(HttpMessageHandler handler) => _handler = handler;
        public HttpClient CreateClient(string name) => new(_handler, disposeHandler: false);
    }
}
