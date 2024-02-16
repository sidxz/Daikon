
using CQRS.Core.Command;
using MediatR;

namespace Project.Application.Features.Commands.DeleteProject
{
    public class DeleteProjectCommand : BaseCommand, IRequest<Unit>
    {
        public Guid StrainId { get; set; }
        public string Name { get; set; }


    }
}