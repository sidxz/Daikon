
using MediatR;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.Orgs.UpdateOrg
{
    public class UpdateOrgCommand : IRequest<AppOrg>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public bool? IsInternal { get; set; }
    }
}