using Daikon.ApiHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using UserStore.API;
using Xunit;

namespace UserStore.Test;

public class AppUserResolverTests
{
    [Fact]
    public void UserStore_resolves_users_in_process_instead_of_calling_itself()
    {
        // The real UserStore startup; the default resolver would call UserStore over HTTP, i.e. itself
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(host =>
        {
            host.UseSetting("OIDCProvider", "EntraID");
            host.UseSetting("EntraID:Instance", "https://login.microsoftonline.com/");
            host.UseSetting("EntraID:Domain", "example.onmicrosoft.com");
            host.UseSetting("EntraID:TenantId", "11111111-1111-1111-1111-111111111111");
            host.UseSetting("EntraID:ClientId", "22222222-2222-2222-2222-222222222222");
            host.UseSetting("EntraID:Audience", "api://22222222-2222-2222-2222-222222222222");
        });

        using var scope = factory.Services.CreateScope();
        Assert.IsType<LocalAppUserResolver>(scope.ServiceProvider.GetRequiredService<IAppUserResolver>());
    }
}
