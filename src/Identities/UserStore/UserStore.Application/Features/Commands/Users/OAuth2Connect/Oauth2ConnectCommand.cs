
using MediatR;

namespace UserStore.Application.Features.Commands.Users.OAuth2Connect
{
    public class Oauth2ConnectCommand : IRequest<Unit>
    {
        public string OIDCSub { get; set; }
        public Guid EntraObjectId { get; set; }
        public string Email { get; set; }
    }
}