
using CQRS.Core.Command;
using MediatR;

namespace Gene.Application.Features.Command.DeleteProteinActivityAssay
{
    public class DeleteProteinActivityAssayCommand : BaseCommand, IRequest<Unit>
    {
        public Guid GeneId { get; set; }
        public Guid ProteinActivityAssayId { get; set; }
    }
}