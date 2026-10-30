using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Xunit;

namespace Daikon.ApiHost.Test
{
    // Every service's controllers go through the versioning that AddDaikonApiDefaults sets up: URL segment,
    // x-api-version header and media-type parameter, default 1.0. The controllers below mirror the services'
    // own (V1/V2 pairs on api/v{version:apiVersion}/[controller], one unversioned route like EventManagement's),
    // so this exercises real MVC dispatch rather than a minimal API with a literal "v2" in its path.
    public class ApiVersioningTests
    {
        [Theory]
        [InlineData("/api/v1/thing", "v1")]
        [InlineData("/api/v2/thing", "v2")]
        [InlineData("/api/v2/thing/extra", "extra")]  // [MapToApiVersion]
        [InlineData("/api/v2/my-thing", "my-thing")]  // slugified [controller]
        public async Task Url_segment_selects_the_controller_for_that_version(string url, string expected)
        {
            var client = await StartService();
            var response = await client.SendAsync(Get(url));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expected, await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Unknown_version_is_refused_and_keeps_the_security_headers()
        {
            var client = await StartService();
            var response = await client.SendAsync(Get("/api/v9/thing"));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Contains("no-store", response.Headers.CacheControl!.ToString());
            Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
        }

        [Fact]
        public async Task Header_and_media_type_select_the_version_on_an_unversioned_route()
        {
            var client = await StartService();

            // No version anywhere: the default 1.0 applies, and the controller only serves 2.0
            Assert.NotEqual(HttpStatusCode.OK, (await client.SendAsync(Get("/api/replay"))).StatusCode);

            var byHeader = Get("/api/replay");
            byHeader.Headers.Add("x-api-version", "2.0");
            var response = await client.SendAsync(byHeader);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("replay", await response.Content.ReadAsStringAsync());

            var byMediaType = Get("/api/replay");
            byMediaType.Headers.TryAddWithoutValidation("Accept", "application/json;x-api-version=2.0");
            response = await client.SendAsync(byMediaType);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("replay", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Supported_and_deprecated_versions_are_reported()
        {
            var client = await StartService();
            var response = await client.SendAsync(Get("/api/v2/thing"));
            Assert.Equal("2.0", response.Headers.GetValues("api-supported-versions").Single());  // deprecated versions are reported separately
            Assert.Equal("1.0", response.Headers.GetValues("api-deprecated-versions").Single());
        }

        private static HttpRequestMessage Get(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.TryAddWithoutValidation("Authorization", "Test user");
            return request;
        }

        private static async Task<HttpClient> StartService()
        {
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.UseTestServer();
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OIDCProvider"] = "EntraID",
                ["EntraID:Instance"] = "https://login.microsoftonline.com/",
                ["EntraID:Domain"] = "example.onmicrosoft.com",
                ["EntraID:TenantId"] = "11111111-1111-1111-1111-111111111111",
                ["EntraID:ClientId"] = "22222222-2222-2222-2222-222222222222",
                ["EntraID:Audience"] = "api://22222222-2222-2222-2222-222222222222",
            });
            builder.Services.AddDaikonApiDefaults(builder.Configuration);
            builder.Services.AddControllers().AddApplicationPart(typeof(ApiVersioningTests).Assembly);
            // Token validation is AuthenticationTests' subject; a stub scheme stands in so the routes are reachable
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Test";
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            }).AddScheme<AuthenticationSchemeOptions, StubTokenHandler>("Test", null);
            builder.Services.AddScoped<IAppUserResolver, StubUserResolver>();

            var app = builder.Build();
            app.UseDaikonApiDefaults();
            await app.StartAsync();
            return app.GetTestClient();
        }

        private class StubTokenHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public StubTokenHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
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

        private class StubUserResolver : IAppUserResolver
        {
            private static readonly AppUserIdentity User = new(Guid.NewGuid(), Guid.NewGuid(), "Versioning Test User");
            public Task<AppUserIdentity?> Resolve(HttpContext context) => Task.FromResult<AppUserIdentity?>(User);
        }
    }
}

namespace Daikon.ApiHost.Test.VersionedControllers.V1
{
    [ApiController, ApiVersion("1.0", Deprecated = true), Route("api/v{version:apiVersion}/[controller]")]
    public class ThingController : ControllerBase
    {
        [HttpGet] public string Get() => "v1";
    }
}

namespace Daikon.ApiHost.Test.VersionedControllers.V2
{
    [ApiController, ApiVersion("2.0"), Route("api/v{version:apiVersion}/[controller]")]
    public class ThingController : ControllerBase
    {
        [HttpGet] public string Get() => "v2";
        [HttpGet("extra"), MapToApiVersion("2.0")] public string Extra() => "extra";
    }

    [ApiController, ApiVersion("2.0"), Route("api/v{version:apiVersion}/[controller]")]
    public class MyThingController : ControllerBase
    {
        [HttpGet] public string Get() => "my-thing";
    }

    // EventManagement's ReplayController style: fixed route, version only from the header or media type
    [ApiController, ApiVersion("2.0"), Route("api/replay")]
    public class ReplayController : ControllerBase
    {
        [HttpGet, MapToApiVersion("2.0")] public string Get() => "replay";
    }
}
