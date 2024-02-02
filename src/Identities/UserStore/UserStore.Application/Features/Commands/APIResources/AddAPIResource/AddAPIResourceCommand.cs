
using MediatR;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.APIResources.AddAPIResource
{
    public class AddAPIResourceCommand : IRequest<APIResource>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Service { get; set; }
        public required string Method { get; set; }
        public required string Endpoint { get; set; }
        public List<Guid> AttachedAppRoles { get; set; }
    }
}