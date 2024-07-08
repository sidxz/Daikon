using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace SimpleGW.OIDCProviders
{
    public static class KeyCloak
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            // KeyCloak Configuration
            var keyCloakConfig = configuration.GetSection("KeyCloak");

            if (!keyCloakConfig.Exists()) return;

            // Dump configuration to console for debugging purposes
            Console.WriteLine($"KeyCloak Instance: {keyCloakConfig["Instance"]}");
            Console.WriteLine($"KeyCloak Realm: {keyCloakConfig["Realm"]}");
            Console.WriteLine($"KeyCloak Authority: {keyCloakConfig["Authority"]}");
            Console.WriteLine($"KeyCloak Audience: {keyCloakConfig["Audience"]}");
            Console.WriteLine($"KeyCloak ClientId: {keyCloakConfig["ClientId"]}");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Bind configuration to options
                options.Authority = keyCloakConfig["Authority"];
                options.Audience = keyCloakConfig["Audience"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, // Enable issuer validation
                    ValidIssuer = keyCloakConfig["Authority"],
                    ValidateAudience = true,
                    ValidAudience = keyCloakConfig["Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    NameClaimType = ClaimTypes.NameIdentifier
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine(" -------- Authentication failed.---------");
                        Console.WriteLine(context.Exception);

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine(" ------ Token validated. -----");
                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        if (claimsIdentity != null)
                        {
                            Console.WriteLine($"ClaimsIdentity is authenticated: {claimsIdentity.IsAuthenticated}");
                            // foreach (var claim in claimsIdentity.Claims)
                            // {
                            //     Console.WriteLine($"Claim type: {claim.Type}, value: {claim.Value}");
                            // }
                        }
                        return Task.CompletedTask;
                    },
                    // Other events...
                };
            });
            services.AddAuthorization(options =>
           {
               options.DefaultPolicy = new AuthorizationPolicyBuilder()
                   .RequireAuthenticatedUser()
                   .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                   .Build();
           });
        }
    }
}
