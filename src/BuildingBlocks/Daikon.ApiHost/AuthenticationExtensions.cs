using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Daikon.ApiHost;

public static class AuthenticationExtensions
{
    // Used by SimpleGW and every service, so each hop validates the forwarded bearer token by the same rules.
    // Missing or unknown provider config fails startup rather than leaving a service unauthenticated.
    public static IServiceCollection AddDaikonAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["OIDCProvider"];
        switch (provider)
        {
            case "EntraID":
                RequireKeys(configuration.GetSection("EntraID"), "Instance", "Domain", "TenantId", "Audience", "ClientId");
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApi(configuration, "EntraID");
                // Microsoft.Identity.Web accepts a token with either "scp" or "roles". DAIKON serves signed-in users only, so a
                // delegated access token ("scp") is required: an ID token of the same app registration carries "roles" but no "scp".
                services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Events ??= new JwtBearerEvents();
                    var validated = options.Events.OnTokenValidated;
                    options.Events.OnTokenValidated = async context =>
                    {
                        if (validated is not null)
                        {
                            await validated(context);
                        }
                        if (context.Result is null && context.Principal?.HasClaim(c => c.Type is ClaimConstants.Scp or ClaimConstants.Scope && !string.IsNullOrWhiteSpace(c.Value)) != true)
                        {
                            context.Fail("Not a delegated access token (no scp claim)");
                        }
                    };
                });
                break;

            case "KeyCloak":
                var keyCloak = configuration.GetSection("KeyCloak");
                RequireKeys(keyCloak, "Authority", "Audience");
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Authority = keyCloak["Authority"];
                        options.Audience = keyCloak["Audience"];
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = keyCloak["Authority"],
                            ValidateAudience = true,
                            ValidAudience = keyCloak["Audience"],
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            NameClaimType = ClaimTypes.NameIdentifier
                        };
                        options.Events = new JwtBearerEvents
                        {
                            // KeyCloak signs ID, logout and access tokens with the same realm key, and an ID token's
                            // audience is the client id. Only access tokens (payload "typ": "Bearer") may call the API.
                            OnTokenValidated = context =>
                            {
                                if (context.SecurityToken is not JsonWebToken token
                                    || !token.TryGetPayloadValue<string>("typ", out var typ) || typ != "Bearer")
                                {
                                    context.Fail("Not a KeyCloak access token");
                                }
                                return Task.CompletedTask;
                            }
                        };
                    });
                break;

            default:
                throw new InvalidOperationException($"OIDCProvider must be 'EntraID' or 'KeyCloak' (got '{provider}').");
        }

        // Set a single handler instead of replacing Events, so Microsoft.Identity.Web's own token checks stay in place.
        services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Events ??= new JwtBearerEvents();
            options.Events.OnAuthenticationFailed = context =>
            {
                context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger(nameof(AuthenticationExtensions))
                    .LogWarning("Bearer token rejected: {Reason}", context.Exception.Message);
                return Task.CompletedTask;
            };
        });

        return services;
    }

    private static void RequireKeys(IConfigurationSection section, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (string.IsNullOrWhiteSpace(section[key]))
            {
                throw new InvalidOperationException($"{section.Path}:{key} configuration is missing or empty.");
            }
        }
    }
}
