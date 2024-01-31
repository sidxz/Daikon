
using MediatR;

namespace UserStore.Application.Features.Commands.Orgs.DeleteOrg
{
    public class DeleteOrgCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        
    }
}