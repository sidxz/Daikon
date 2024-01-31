
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace OcelotApiGw.OIDCProviders
{
    public static class KeyCloak
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            // Microsoft Entra ID Configuration
            var keyCloakConfig = configuration.GetSection("KeyCloak");

            if (!keyCloakConfig.Exists()) return;

            
            // dump configuration to console
            // Console.WriteLine($"KeyCloak Instance: {keyCloakConfig["Instance"]}");
            // Console.WriteLine($"KeyCloak Domain: {keyCloakConfig["Domain"]}");
            // Console.WriteLine($"KeyCloak TenantId: {keyCloakConfig["TenantId"]}");
            // Console.WriteLine($"KeyCloak Audience: {keyCloakConfig["Audience"]}");
            // Console.WriteLine($"KeyCloak ClientId: {keyCloakConfig["ClientId"]}");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("KeyCloak", options =>
            {
                configuration.Bind("KeyCloak", options);
                options.Authority = keyCloakConfig["Authority"];
                options.Audience = keyCloakConfig["Audience"];
                options.TokenValidationParameters.ValidateIssuer = false;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine(" -------- Authentication failed.---------", context.Exception);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine(" ------ Token validated. -----");
                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        if (claimsIdentity != null)
                        {
                            Console.WriteLine($"ClaimsIdentity is authenticated: {claimsIdentity.IsAuthenticated}");
                            foreach (var claim in claimsIdentity.Claims)
                            {
                                Console.WriteLine($"Claim type: {claim.Type}, value: {claim.Value}");
                            }
                        }
                        return Task.CompletedTask;
                    },
                    // Other events...
                };
            });
        }
    }
}