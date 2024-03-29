
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace OcelotApiGw.OIDCProviders
{
    public static class MicrosoftEntraID
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            // Microsoft Entra ID Configuration
            var entraIdConfig = configuration.GetSection("EntraID");

            if (!entraIdConfig.Exists()) return;
            
            // dump configuration to console
            // Console.WriteLine($"EntraID Instance: {entraIdConfig["Instance"]}");
            // Console.WriteLine($"EntraID Domain: {entraIdConfig["Domain"]}");
            // Console.WriteLine($"EntraID TenantId: {entraIdConfig["TenantId"]}");
            // Console.WriteLine($"EntraID Audience: {entraIdConfig["Audience"]}");
            // Console.WriteLine($"EntraID ClientId: {entraIdConfig["ClientId"]}");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("EntraID", options =>
            {
                configuration.Bind("EntraID", options);
                options.Authority = $"{entraIdConfig["Instance"]}{entraIdConfig["TenantId"]}";
                options.Audience = entraIdConfig["Audience"];
                options.TokenValidationParameters.ValidateAudience = false;
                

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine(" -------- Authentication failed.---------", context.Exception);
                        Console.WriteLine($"Exception: {context.Exception.Message}");
                        Console.WriteLine($"--------------------------End of Authentication Failed--------------------------");
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