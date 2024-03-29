
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace SimpleGW.OIDCProviders
{
    public static class MicrosoftEntraID
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            // Microsoft Entra ID Configuration
            var entraIdConfig = configuration.GetSection("EntraID");
            if (!entraIdConfig.Exists())
            {
                logger.LogWarning("EntraID configuration section is missing.");
                return;
            }

            /* Dump configuration to console for debugging */
            // Console.WriteLine($"EntraID Instance: {entraIdConfig["Instance"]}");
            // Console.WriteLine($"EntraID Domain: {entraIdConfig["Domain"]}");
            // Console.WriteLine($"EntraID TenantId: {entraIdConfig["TenantId"]}");
            // Console.WriteLine($"EntraID Audience: {entraIdConfig["Audience"]}");
            // Console.WriteLine($"EntraID ClientId: {entraIdConfig["ClientId"]}");
            
            var requiredConfigs = new[] { "Instance", "Domain", "TenantId", "Audience", "ClientId" };
            foreach (var configKey in requiredConfigs)
            {
                if (string.IsNullOrEmpty(entraIdConfig[configKey]))
                {
                    logger.LogWarning($"EntraID configuration key '{configKey}' is missing or empty.");
                    throw new InvalidOperationException($"EntraID configuration key '{configKey}' is missing or empty.");
                }
            }
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                configuration.Bind(options);
                options.Authority = $"{entraIdConfig["Instance"]}{entraIdConfig["TenantId"]}";
                options.Audience = entraIdConfig["Audience"];
                options.TokenValidationParameters.ValidateAudience = false;

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        logger.LogError($"Authentication failed: {context.Exception}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        logger.LogDebug("Token validated.");
                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        if (claimsIdentity != null)
                        {
                            logger.LogDebug($"ClaimsIdentity is authenticated: {claimsIdentity.IsAuthenticated}");
                            foreach (var claim in claimsIdentity.Claims)
                            {
                                //Console.WriteLine($"Claim type: {claim.Type}, value: {claim.Value}");
                            }
                        }
                        return Task.CompletedTask;
                    },
                };
            });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("EntraID")
                    .Build();
            });
        }
    }
}