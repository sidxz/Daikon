
using MediatR;

namespace UserStore.Application.Features.Commands.APIResources.DeleteAPIResource
{
    public class DeleteAPIResourceCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}