using CQRS.Core.Command;
using MediatR;

namespace DocuStore.Application.Features.Commands.DeleteParsedDoc
{
    public class DeleteParsedDocCommand : BaseCommand, IRequest<Unit>
    {
        
    }
}