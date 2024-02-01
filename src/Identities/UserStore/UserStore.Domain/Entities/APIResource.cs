

namespace UserStore.Domain.Entities
{
    public class APIResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Service { get; set; }
        public required string Endpoint { get; set; }
        public List<Guid> AttachedAppRoles { get; set; }

    }
}