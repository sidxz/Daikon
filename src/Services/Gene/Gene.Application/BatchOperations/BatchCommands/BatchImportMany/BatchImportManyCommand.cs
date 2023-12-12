
using CQRS.Core.Command;
using CQRS.Core.Domain;
using Gene.Application.BatchOperations.BatchCommands.BatchImportOne;
using Gene.Domain.Entities;
using MediatR;

namespace Gene.Application.BatchOperations.BatchCommands.BatchImportMany
{
    public class BatchImportManyCommand : BaseCommand, IRequest<Unit>
    {
        
        public List <BatchImportOneCommand> Batch { get; set; }
        
    }
}