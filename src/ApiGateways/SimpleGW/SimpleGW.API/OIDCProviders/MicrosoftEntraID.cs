using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace SimpleGW.OIDCProviders
{
    public static class MicrosoftEntraID
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            // Microsoft Entra ID Configuration
            var entraIdConfig = configuration.GetSection("AzureAd"); // Use AzureAd section as recommended
            if (!entraIdConfig.Exists())
            {
                logger.LogWarning("AzureAd configuration section is missing.");
                return;
            }

            /* Dump configuration to console for debugging */
            Console.WriteLine($"AzureAd Instance: {entraIdConfig["Instance"]}");
            Console.WriteLine($"AzureAd Domain: {entraIdConfig["Domain"]}");
            Console.WriteLine($"AzureAd TenantId: {entraIdConfig["TenantId"]}");
            Console.WriteLine($"AzureAd Audience: {entraIdConfig["Audience"]}");
            Console.WriteLine($"AzureAd ClientId: {entraIdConfig["ClientId"]}");
            Console.WriteLine($"AzureAd Issuer: {entraIdConfig["Issuer"]}");

            var requiredConfigs = new[] { "Instance", "Domain", "TenantId", "Audience", "ClientId" };
            foreach (var configKey in requiredConfigs)
            {
                if (string.IsNullOrEmpty(entraIdConfig[configKey]))
                {
                    logger.LogWarning($"AzureAd configuration key '{configKey}' is missing or empty.");
                    throw new InvalidOperationException($"AzureAd configuration key '{configKey}' is missing or empty.");
                }
            }

            // Configure Microsoft Identity Web to use Azure AD
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApi(configuration, "AzureAd");

            // Configure event handlers and default authorization policy
            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
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
                                Console.WriteLine($"Claim type: {claim.Type}, value: {claim.Value}");
                            }
                        }
                        return Task.CompletedTask;
                    }
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
