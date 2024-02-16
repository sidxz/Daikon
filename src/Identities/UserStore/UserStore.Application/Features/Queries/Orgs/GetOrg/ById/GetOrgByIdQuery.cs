
using MediatR;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Queries.Orgs.GetOrg.ById
{
    public class GetOrgByIdQuery : IRequest<AppOrg>
    {
        public Guid Id { get; set; }
    }
}