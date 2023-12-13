
using CQRS.Core.Command;
using MediatR;

namespace Gene.Application.Features.Command.DeleteResistanceMutation
{
    public class DeleteResistanceMutationCommand : BaseCommand, IRequest<Unit>
    {
        public Guid GeneId { get; set; }
        public Guid ResistanceMutationId { get; set; }
    }
}