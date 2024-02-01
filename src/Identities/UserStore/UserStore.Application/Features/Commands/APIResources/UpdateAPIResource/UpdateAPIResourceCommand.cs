
using MediatR;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.APIResources.UpdateAPIResource
{
    public class UpdateAPIResourceCommand : IRequest<APIResource>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Service { get; set; }
        public required string Endpoint { get; set; }
        public List<Guid> AttachedAppRoles { get; set; }
    }
}