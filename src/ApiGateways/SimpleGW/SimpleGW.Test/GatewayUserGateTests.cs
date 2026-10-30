using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Daikon.ApiHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace SimpleGW.Test;

// A valid Entra token is not enough: the gateway, like every service, only forwards for a DAIKON user
public class GatewayUserGateTests
{
    [Fact]
    public async Task A_valid_token_for_someone_who_is_not_a_daikon_user_is_refused_at_the_gateway()
    {
        var service = new CountingService();
        using var gateway = Gateway<TestGateway.NotAUser>(service);

        var response = await gateway.CreateClient().SendAsync(Authenticated("/v2/gene"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("no-store, max-age=0", response.Headers.CacheControl!.ToString());
        Assert.Equal(0, service.Calls); // never reached a service
    }

    [Fact]
    public async Task A_daikon_user_is_forwarded()
    {
        var service = new CountingService();
        using var gateway = Gateway<TestGateway.KnownUser>(service);

        var response = await gateway.CreateClient().SendAsync(Authenticated("/v2/gene"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, service.Calls);
    }

    private static WebApplicationFactory<Program> Gateway<TResolver>(CountingService service) where TResolver : class, IAppUserResolver =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(host => TestGateway.Configure(host, services =>
        {
            services.AddScoped<IAppUserResolver, TResolver>();
            services.AddHttpClient("SimpleGWClient").ConfigurePrimaryHttpMessageHandler(() => service);
        }));

    private static HttpRequestMessage Authenticated(string path)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TestGateway.Token());
        return request;
    }

    private class CountingService : HttpMessageHandler
    {
        public int Calls;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.RequestUri!.AbsolutePath.EndsWith("/health")) // the gateway's health poller shares this client
            {
                Calls++;
            }
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"genes\":[]}", Encoding.UTF8, "application/json"),
            });
        }
    }
}
