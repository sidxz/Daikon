using System.Security.Claims;
using Daikon.ApiHost;
using MediatR;
using Microsoft.Identity.Web;
using UserStore.Application.Features.Commands.Users.ValidateUserAccess;

namespace UserStore.API
{
    // UserStore owns the user records, so it resolves the token's owner in-process.
    // The default resolver calls UserStore over HTTP, which from here would call itself forever.
    public class LocalAppUserResolver : IAppUserResolver
    {
        private readonly IMediator _mediator;

        public LocalAppUserResolver(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<AppUserIdentity?> Resolve(HttpContext context)
        {
            ValidateUserAccessResponse response;
            try
            {
                // Identifiers come from the validated token only
                response = await _mediator.Send(new ValidateUserAccessCommand
                {
                    EntraObjectId = context.User.GetObjectId(),
                    OIDCSub = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    Email = context.User.FindFirst(ClaimTypes.Email)?.Value
                }, context.RequestAborted);
            }
            catch (InvalidOperationException)
            {
                // Refused account linking, e.g. the account is already linked to a different Entra ID.
                // Not a valid user (401), as the old /user/validate endpoint treated it; the handler has logged why.
                return null;
            }

            return response.IsValid
                ? new AppUserIdentity(response.AppUserId, response.AppOrgId, $"{response.FirstName} {response.LastName}")
                : null;
        }
    }
}
