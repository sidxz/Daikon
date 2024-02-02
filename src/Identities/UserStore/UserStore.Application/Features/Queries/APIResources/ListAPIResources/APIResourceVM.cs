
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Queries.APIResources.ListAPIResources
{
    public class APIResourceVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Service { get; set; }
        public required string Method { get; set; }
        public required string Endpoint { get; set; }
        public List<Guid> AttachedAppRoles { get; set; }
        public List<AppRole> AttachedAppRolesExpanded { get; set; }
    }
}