
using MediatR;

namespace UserStore.Application.Features.Commands.Users.OAuth2Connect
{
    public class OAuth2ConnectCommand : IRequest<Unit>
    {
        public string OIDCSub { get; set; }
        public string EntraObjectId { get; set; }
        public string Email { get; set; }
    }
}