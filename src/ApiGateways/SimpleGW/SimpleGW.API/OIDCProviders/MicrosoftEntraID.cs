
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

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
            Console.WriteLine($"EntraID Instance: {entraIdConfig["Instance"]}");
            Console.WriteLine($"EntraID Domain: {entraIdConfig["Domain"]}");
            Console.WriteLine($"EntraID TenantId: {entraIdConfig["TenantId"]}");
            Console.WriteLine($"EntraID Audience: {entraIdConfig["Audience"]}");
            Console.WriteLine($"EntraID ClientId: {entraIdConfig["ClientId"]}");
            Console.WriteLine($"EntraID Issuer: {entraIdConfig["Issuer"]}");

            var requiredConfigs = new[] { "Instance", "Domain", "TenantId", "Audience", "ClientId" };
            foreach (var configKey in requiredConfigs)
            {
                if (string.IsNullOrEmpty(entraIdConfig[configKey]))
                {
                    logger.LogWarning($"EntraID configuration key '{configKey}' is missing or empty.");
                    throw new InvalidOperationException($"EntraID configuration key '{configKey}' is missing or empty.");
                }
            }
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                configuration.Bind(options);
                options.Authority = $"{entraIdConfig["Instance"]}{entraIdConfig["TenantId"]}/v2.0/";
                options.Audience = entraIdConfig["Audience"];
                // options.TokenValidationParameters.ValidateAudience = false;
                // Enable issuer validation explicitly
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidIssuer = entraIdConfig["Issuer"],
                    ValidateAudience = false,
                    ValidAudience = entraIdConfig["Audience"],
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = false,
                    ClockSkew = TimeSpan.Zero
                };

                Console.WriteLine("EntraID Configuration: {options}");
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(options));


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