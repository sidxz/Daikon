using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging.Abstractions;
using SimpleGW.API.Middlewares;
using Xunit;

namespace SimpleGW.Test;

public class RequestForwardingTests
{
    [Fact]
    public async Task Only_allowlisted_headers_cross_the_gateway_in_either_direction()
    {
        var service = new StubService(response =>
        {
            response.Headers.TryAddWithoutValidation("Server", "Kestrel");
            response.Headers.TryAddWithoutValidation("X-Internal-Debug", "gene-api:80");
            response.Headers.TryAddWithoutValidation("Access-Control-Allow-Origin", "*");
            response.Headers.TryAddWithoutValidation("api-supported-versions", "1.0, 2.0");
            response.Content = new StringContent("{}", Encoding.UTF8, "application/problem+json");
        });

        var context = new DefaultHttpContext { TraceIdentifier = "gateway-trace" };
        context.Items["Microservice"] = "http://gene-api";
        context.Request.Method = "DELETE";
        context.Request.Path = "/v2/hit-collection/1/hit-batch";
        context.Request.Headers.Authorization = "Bearer t";
        context.Request.Headers["Cookie"] = "session=1";
        context.Request.Headers["X-Forwarded-For"] = "203.0.113.9";
        context.Request.Headers["AppUser-Id"] = Guid.NewGuid().ToString();
        context.Request.Headers["trace-id"] = "client-chosen";
        context.Request.Headers["Connection"] = "keep-alive, Upgrade";
        context.Request.ContentType = "application/json; charset=utf-8";
        context.Request.Body = new MemoryStream("[1]"u8.ToArray());
        context.Request.ContentLength = 3;
        context.Features.Set<IHttpRequestBodyDetectionFeature>(new HasBody()); // as Kestrel reports it
        context.Response.Body = new MemoryStream();

        await new RequestForwardingMiddleware(_ => Task.CompletedTask, new StubFactory(service), NullLogger<RequestForwardingMiddleware>.Instance)
            .InvokeAsync(context);

        // Client → service: token, gateway's trace id, the body (even on DELETE) and its exact Content-Type
        var sent = service.Request!;
        Assert.Equal("http://gene-api/api/v2/hit-collection/1/hit-batch", sent.RequestUri!.ToString());
        Assert.Equal(new[] { "Authorization", "trace-id" }, sent.Headers.Select(h => h.Key).OrderBy(k => k));
        Assert.Equal("gateway-trace", sent.Headers.GetValues("trace-id").Single());
        Assert.Equal("[1]", service.Body);
        Assert.Equal("application/json; charset=utf-8", service.ContentType);

        // Service → client: content type and API metadata only
        Assert.Equal(200, context.Response.StatusCode);
        Assert.Equal("application/problem+json; charset=utf-8", context.Response.ContentType);
        Assert.Equal("1.0, 2.0", context.Response.Headers["api-supported-versions"].ToString());
        Assert.False(context.Response.Headers.ContainsKey("Server"));
        Assert.False(context.Response.Headers.ContainsKey("X-Internal-Debug"));
        Assert.False(context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"));
    }

    private class HasBody : IHttpRequestBodyDetectionFeature
    {
        public bool CanHaveBody => true;
    }

    private class StubService : HttpMessageHandler
    {
        private readonly Action<HttpResponseMessage> _respond;
        public HttpRequestMessage? Request { get; private set; }
        public string? Body { get; private set; }
        public string? ContentType { get; private set; }

        public StubService(Action<HttpResponseMessage> respond) => _respond = respond;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            Body = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            ContentType = request.Content?.Headers.ContentType?.ToString();
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            _respond(response);
            return response;
        }
    }

    private class StubFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler;
        public StubFactory(HttpMessageHandler handler) => _handler = handler;
        public HttpClient CreateClient(string name) => new(_handler, disposeHandler: false);
    }
}
