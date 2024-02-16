
using MediatR;

namespace UserStore.Application.Features.Commands.Users.ValidateUserAccess
{
    public class ValidateUserAccessCommand : IRequest<ValidateUserAccessResponse>
    {
        public string? OIDCSub { get; set; }
        public string? EntraObjectId { get; set; }
        public string? Email { get; set; }
    }

}