
using MediatR;

namespace UserStore.Application.Features.Commands.Users.ValidateUserAccess
{
    public class ValidateUserAccessCommand : IRequest<Unit>
    {
        public string OIDCSub { get; set; }
        public Guid EntraObjectId { get; set; }
        public string Email { get; set; }
    }

}