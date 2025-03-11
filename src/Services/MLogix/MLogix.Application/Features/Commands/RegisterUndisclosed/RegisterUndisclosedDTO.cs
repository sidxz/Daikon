
using CQRS.Core.Domain;

namespace MLogix.Application.Features.Commands.RegisterUndisclosed
{
    public class RegisterUndisclosedDTO : VMMeta
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public bool WasAlreadyRegistered { get; set; }
    }
}